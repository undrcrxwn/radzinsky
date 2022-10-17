using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.AuthorizationResults;
using Radzinsky.Domain.Enumerations;
using Radzinsky.Domain.Models.Entities;
using Radzinsky.Persistence;
using Radzinsky.Persistence.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;
    
    public AuthorizationService(
        ITelegramBotClient bot,
        ApplicationDbContext dbContext)
    {
        _bot = bot;
        _dbContext = dbContext;
    }

    public async Task<AuthorizationResult> AuthorizeAsync(long userId, long chatId, ChatMemberPermissions permission)
    {
        var memberDto = await _bot.GetChatMemberAsync(chatId, userId);
        if (memberDto.Status is ChatMemberStatus.Creator or ChatMemberStatus.Administrator)
            return new Authorized(true);

        var chat = await _dbContext.Chats.FindOrAddAsync(chatId, () => new Chat(chatId));
        
        var member = await _dbContext.ChatMembers.FindAsync(userId);
        var role = member?.Role ?? chat.DefaultRole;

        return Authorize(role, chat.Roles, permission);
    }

    public async Task<AuthorizationResult> AuthorizeAgainstAsync(long userId, long targetId, long chatId, ChatMemberPermissions permission)
    {
        var targetMemberDto = await _bot.GetChatMemberAsync(chatId, targetId);
        if (targetMemberDto.Status is ChatMemberStatus.Creator or ChatMemberStatus.Administrator)
            return new FailedAgainstSuperadmin();
        
        var userMemberDto = await _bot.GetChatMemberAsync(chatId, userId);
        if (userMemberDto.Status is ChatMemberStatus.Creator or ChatMemberStatus.Administrator)
            return new Authorized(true);

        var chat = await _dbContext.Chats.FindOrAddAsync(chatId, () => new Chat(chatId));
        
        var targetMember = await _dbContext.ChatMembers.FindAsync(targetId);
        var targetRole = targetMember?.Role ?? chat.DefaultRole;
        
        var userMember = await _dbContext.ChatMembers.FindAsync(userId);
        var userRole = userMember?.Role ?? chat.DefaultRole;

        var result = Authorize(userRole, chat.Roles, permission);
        if (result is not Success)
            return result;

        if (targetRole is null)
            return new Undefined();
        
        return userRole!.Priority > targetRole.Priority
            ? new Authorized(false)
            : new PriorityDifference(userRole.Priority, targetRole.Priority);
    }

    private AuthorizationResult Authorize(Role? role, IEnumerable<Role> rolesHierarchy, ChatMemberPermissions permission)
    {
        if (role is null)
            return new Undefined();

        var rolesHierarchyHasPermission = rolesHierarchy
            .Where(x => x.Priority <= role.Priority)
            .Any(x => x.Permissions.Contains(permission));
        
        return rolesHierarchyHasPermission
            ? new Authorized(false)
            : new NoPermission();
    }
}