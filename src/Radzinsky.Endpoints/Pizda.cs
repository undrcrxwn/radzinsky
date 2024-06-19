using Radzinsky.Framework;
using Radzinsky.Framework.Routing;
using Radzinsky.Framework.Routing.RegEx;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Radzinsky.Endpoints;

[RegExPatterns(@"(?i)^(?:.* )?да[^\w\s]*$")]
public class Pizda(ITelegramBotClient bot) : IEndpoint
{
    public async Task HandleAsync(Update update, Route route, CancellationToken cancellationToken) =>
        await bot.SendTextMessageAsync(update.Message!.Chat, "пизда", cancellationToken: cancellationToken);
}