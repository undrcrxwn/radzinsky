using Radzinsky.Framework;
using Radzinsky.Framework.Exceptions;
using Radzinsky.Framework.Routing;
using Radzinsky.Framework.Routing.RegEx;
using Radzinsky.Framework.Routing.StringDistance;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Radzinsky.Endpoints;

[RegExPatterns(
    @"(?i)^\/echo(?:@radzinsky_bot)? (.+)$",
    @"(?i)^\/эхо(?:@radzinsky_bot)? (.+)$",
    @"(?i)^\/repeat(?:@radzinsky_bot)? (.+)$",
    @"(?i)^\/повтори(?:@radzinsky_bot)? (.+)$")]
[StringDistanceAliases("эхо", "скажи", "скажи что", "повтори", "повторяй за мной")]
public class Echo(ITelegramBotClient bot) : IEndpoint
{
    public async Task HandleAsync(Update update, Route route, CancellationToken cancellationToken)
    {
        var response = route switch
        {
            RegExRoute regExRoute => regExRoute.Groups[1].Value,
            StringDistanceRoute stringDistanceRoute => stringDistanceRoute.TailSegment.ToString(),
            _ => throw new UnsupportedRouteException()
        };

        if (string.IsNullOrWhiteSpace(response))
            return;

        await bot.SendTextMessageAsync(
            chatId: update.Message!.Chat,
            text: response,
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);
    }
}