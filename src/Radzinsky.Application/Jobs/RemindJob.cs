using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Jobs;

public class RemindJob
{
    private readonly ITelegramBotClient _bot;
    
    public RemindJob(ITelegramBotClient bot) =>
        _bot = bot;
    
    public async Task RemindAsync(long chatId, long userId, string name, string content)
    {
        var mention = $"<a href='tg://user?id={userId}'>{name}</a>";
        await _bot.SendTextMessageAsync(chatId, $"{mention} {content}", ParseMode.Html);
    }
}