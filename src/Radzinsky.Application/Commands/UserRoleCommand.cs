using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Domain.Models.Entities;
using Radzinsky.Persistence;
using Radzinsky.Persistence.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Commands;

public class UserRoleCommand : ICommand
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _dbContext;

    public UserRoleCommand(
        ITelegramBotClient bot,
        ApplicationDbContext dbContext)
    {
        _bot = bot;
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var targetId = context.Message.ReplyTarget is null
            ? context.Update.InteractorUserId!.Value
            : context.Message.ReplyTarget.Sender.Id;
        
        var chat = await _dbContext.Chats.FindOrAddAsync(context.Message.Chat.Id, () => new Chat(context.Update.ChatId!.Value));

        var targetMemberDto = await _bot.GetChatMemberAsync(chat.Id, targetId);
        if (targetMemberDto.Status is ChatMemberStatus.Creator or ChatMemberStatus.Administrator)
        {
            await context.SendTextAsync(context.Resources!.Get("Superadmin"));
            return;
        }
        
        var targetMember = await _dbContext.ChatMembers.FindAsync(context.Update.ChatId, targetId);
        var role = targetMember?.Role ?? chat.DefaultRole;
        var response = role is null
            ? context.Resources!.GetRandom("NoRole")
            : role.Title;
        
        await context.SendTextAsync(response);
    }
}