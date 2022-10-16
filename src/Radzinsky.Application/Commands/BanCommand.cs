using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Commands;

public class BanCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Message.ReplyTarget?.Sender.Id == context.Bot.BotId)
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("CannotBanMe"));
            return;
        }
        
        if (context.Message.IsPrivate)
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("CannotBanInPrivateChat"));
            return;
        }
        
        if (context.Message.ReplyTarget is null)
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("NoReplyTarget"));
            return;
        }

        var sender = await context.Bot.GetChatMemberAsync(
            context.Message.Chat.Id, context.Message.Sender.Id);
        
        var target = await context.Bot.GetChatMemberAsync(
            context.Message.Chat.Id, context.Message.ReplyTarget.Sender.Id);

        if (sender.Status is not ChatMemberStatus.Creator and not ChatMemberStatus.Administrator)
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("NotAnAdmin"));
            return;
        }

        if (target.Status is ChatMemberStatus.Creator or ChatMemberStatus.Administrator)
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("CannotBanAdmin"));
            return;
        }

        await context.Bot.BanChatMemberAsync(
            context.Message.Chat.Id, context.Message.ReplyTarget.Sender.Id);

        var response = context.Resources!.GetRandom(
            "UserBanned", context.Message.ReplyTarget.Sender.FirstName);
        
        await context.ReplyAsync(response);
    }
}