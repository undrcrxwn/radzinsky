using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.DTOs;
using Radzinsky.Application.Models.Resources;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Radzinsky.Application.Models.Contexts;

public class CommandContext : ContextBase<CommandResources>
{
    public MessageDto Message = null!;
    public string Payload = null!;
    
    public CommandContext(
        ITelegramBotClient bot,
        ICheckpointMemoryService checkpoints,
        IReplyMemoryService replies)
        : base(bot, checkpoints, replies) { }
}