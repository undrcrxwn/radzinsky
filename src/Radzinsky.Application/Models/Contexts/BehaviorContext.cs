using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Resources;
using Telegram.Bot;

namespace Radzinsky.Application.Models.Contexts;

public class BehaviorContext : ContextBase<BehaviorResources>
{
    public BehaviorContext(
        ITelegramBotClient bot,
        ICheckpointMemoryService checkpoint,
        IReplyMemoryService replies)
        : base(bot, checkpoint, replies) { }
}