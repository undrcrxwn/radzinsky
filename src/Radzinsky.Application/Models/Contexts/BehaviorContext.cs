using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Resources;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Models.Contexts;

public class BehaviorContext : MessageContext
{
    public string BehaviorTypeName = null!;
    public BehaviorResources? Resources;

    public BehaviorContext(
        ITelegramBotClient bot,
        IInteractionService interaction)
        : base(bot, interaction) { }

    public Checkpoint SetMentionCheckpoint()
    {
        Checkpoint = Interaction.IssueMentionCheckpoint(Message.Sender.Id);
        return Checkpoint;
    }
    
    public override async Task<int> ReplyAsync(
        string text,
        ParseMode? parseMode = null,
        bool? disableWebPagePreview = null,
        string? handlerTypeName = null) =>
        await base.ReplyAsync(text, parseMode, disableWebPagePreview,
            handlerTypeName ?? BehaviorTypeName);
}