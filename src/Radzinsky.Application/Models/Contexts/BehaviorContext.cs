using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.DTOs;
using Radzinsky.Application.Models.Resources;
using Telegram.Bot;

namespace Radzinsky.Application.Models.Contexts;

public class BehaviorContext : ContextBase
{
    public BehaviorResources? Resources;
    

    public BehaviorContext(
        ITelegramBotClient bot,
        ICheckpointMemoryService checkpoint,
        IReplyMemoryService replies)
        : base(bot, checkpoint, replies) { }
}