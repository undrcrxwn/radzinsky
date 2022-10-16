using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.DTOs;
using Radzinsky.Application.Models.Resources;
using Telegram.Bot;

namespace Radzinsky.Application.Models.Contexts;

public class CallbackQueryContext : ContextBase<ResourcesBase>
{
    public CallbackQueryDto Query = null!;
    
    public CallbackQueryContext(
        ITelegramBotClient bot,
        ICheckpointMemoryService checkpoints,
        IReplyMemoryService replies)
        : base(bot, checkpoints, replies) { }
}