using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Commands;

public class BanCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Message.ReplyTarget is null)
        {
            await context.ReplyAsync(context.Resources.Variants["NoReplyTarget"].PickRandom());
            return;
        }

        var sender = await context.Bot.GetChatMemberAsync(
            context.Message.Chat.Id, context.Message.Sender.Id);
        
        var target = await context.Bot.GetChatMemberAsync(
            context.Message.Chat.Id, context.Message.ReplyTarget.Sender.Id);

        if (context.Message.ReplyTarget.Sender.Id == context.Bot.BotId)
        {
            await context.ReplyAsync(context.Resources.Variants["CannotBanMe"].PickRandom());
            return;
        }
        
        if (sender.Status is not ChatMemberStatus.Creator and not ChatMemberStatus.Administrator)
        {
            await context.ReplyAsync(context.Resources.Variants["NotAnAdmin"].PickRandom());
            return;
        }

        if (target.Status is ChatMemberStatus.Creator or ChatMemberStatus.Administrator)
        {
            await context.ReplyAsync(context.Resources.Variants["CannotBanAdmin"].PickRandom());
            return;
        }

        await context.Bot.BanChatMemberAsync(
            context.Message.Chat.Id, context.Message.ReplyTarget.Sender.Id);

        var responseTemplate = context.Resources.Variants["UserBanned"].PickRandom();
        var response = string.Format(responseTemplate, context.Message.ReplyTarget.Sender.FirstName);
        await context.ReplyAsync(response);
    }
}