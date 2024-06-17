using Radzinsky.Framework;
using Radzinsky.Framework.Routing;
using Radzinsky.Framework.Routing.RegEx;
using Radzinsky.Framework.Routing.StringDistance;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Radzinsky.Endpoints;

[RegExPatterns(@"(?i)^\/start(?:@radzinsky_bot)?$")]
[StringDistanceAliases(
    "старт", "начать", "привет", "салют", "здравствуй", "здарова", "дарова", "сап",
    "алё", "алло", "доброе утро", "добрый день", "добрый вечер", "доброго времени суток",
    "ку", "куку", "ку ку")]
public class Start(ITelegramBotClient bot) : IEndpoint
{
    public async Task HandleAsync(Update update, Route route, CancellationToken cancellationToken) =>
        await bot.SendTextMessageAsync(
            chatId: update.Message!.Chat,
            text: "привет!",
            replyToMessageId: update.Message.MessageId,
            cancellationToken: cancellationToken);
}