using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.DTOs;
using Radzinsky.Application.Models.Resources;
using Telegram.Bot;

namespace Radzinsky.Application.Models.Contexts;

public class CallbackQueryContext : ContextBase<ResourcesBase>
{
    public CallbackQueryDto Query = null!;

    private readonly ITelegramBotClient _bot;

    public CallbackQueryContext(
        ITelegramBotClient bot,
        ICheckpointMemoryService checkpoints,
        IReplyMemoryService replies)
        : base(bot, checkpoints, replies) => _bot = bot;

    public async Task ShowAlertAsync(string text) =>
        await _bot.AnswerCallbackQueryAsync(Query.Id, text, true);
    
    public async Task ShowNotificationAsync(string text) =>
        await _bot.AnswerCallbackQueryAsync(Query.Id, text);
}