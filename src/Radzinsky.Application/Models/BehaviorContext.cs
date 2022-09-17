using Radzinsky.Application.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Models;

public class BehaviorContext : MessageContext
{
    public BehaviorResources? Resources;

    public BehaviorContext(
        ITelegramBotClient bot,
        IInteractionService interaction)
        : base(bot, interaction) { }

    public Checkpoint SetMentionCheckpoint()
    {
        Checkpoint = _interaction.IssueMentionCheckpoint(Message.Sender.Id);
        return Checkpoint;
    }
    
    public override async Task<int> ReplyAsync(
        string text,
        ParseMode? parseMode = null,
        bool? disableWebPagePreview = null,
        string? handlerTypeName = null) =>
        await base.ReplyAsync(text, parseMode, disableWebPagePreview,
            handlerTypeName ?? Resources.BehaviorTypeName);
}