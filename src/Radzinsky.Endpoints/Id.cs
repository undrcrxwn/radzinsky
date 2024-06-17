using System.Text;
using Radzinsky.Framework;
using Radzinsky.Framework.Routing;
using Radzinsky.Framework.Routing.RegEx;
using Radzinsky.Framework.Routing.StringDistance;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Endpoints;

[RegExPatterns(@"(?i)^\/id(?:@radzinsky_bot)?$")]
[StringDistanceAliases("айди", "мой айди", "айди чата", "чат")]
public class Id(ITelegramBotClient bot) : IEndpoint
{
    public async Task HandleAsync(Update update, Route route, CancellationToken cancellationToken)
    {
        var response = new StringBuilder();

        if (update.Message!.Chat.Id != update.Message.From!.Id)
            response.AppendLine($"ID чата: `{update.Message.Chat.Id}`");

        response.AppendLine($"Ваш ID: `{update.Message.From.Id}`");
        response.AppendLine($"Мой ID: `{bot.BotId}`");

        if (update.Message.ReplyToMessage?.From is { } messageAuthor && messageAuthor.Id != bot.BotId)
            response.Append($"ID {messageAuthor.FirstName}: `{messageAuthor.Id}`");

        await bot.SendTextMessageAsync(
            chatId: update.Message!.Chat,
            text: response.ToString(),
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }
}