using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Domain.Models.Entities;
using Radzinsky.Persistence;
using Radzinsky.Persistence.Extensions;

namespace Radzinsky.Application.Commands;

public class UserRoleCommand : ICommand
{
    private readonly ApplicationDbContext _dbContext;

    public UserRoleCommand(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var targetId = context.Message.ReplyTarget is null
            ? context.Update.InteractorUserId
            : context.Message.ReplyTarget.Sender.Id;
        
        var chat = await _dbContext.Chats.FindOrAddAsync(context.Message.Chat.Id, () => new Chat(context.Update.ChatId!.Value));
        var targetMember = await _dbContext.ChatMembers.FindAsync(context.Update.ChatId, targetId);

        if (targetMember?.IsChatAdministrator ?? false)
        {
            await context.SendTextAsync(context.Resources!.Get("Superadmin"));
            return;
        }
        
        var role = targetMember?.Role ?? chat.DefaultRole;
        var response = role is null
            ? context.Resources!.GetRandom("NoRole")
            : role.Title;
        
        await context.SendTextAsync(response);
    }
}