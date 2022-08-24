using System.Text;
using MediatR;
using Radzinsky.Bot.Abstractions;
using Radzinsky.Bot.Attributes;
using Radzinsky.Bot.Enumerations;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Radzinsky.Bot.Imperatives;

public record AnalysisRequest(long ChatId, Chat Chat) : IRequest;

[ImperativeCallMapping(ImperativeType.Analysis)]
[ImperativeArgumentParsingStrategy(ImperativeType.Analysis)]
public class AnalysisImperative : IImperative<AnalysisRequest>
{
    private readonly ITelegramBotClient _bot;

    public AnalysisImperative(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public IEnumerable<object>? TryParseArguments(ReadOnlyMemory<char> text)
    {
        return Enumerable.Empty<object>();
    }

    public IBaseRequest MapToRequest(Message context, IEnumerable<object> arguments)
    {
        return new AnalysisRequest(context.Chat.Id, context.Chat);
    }

    public async Task<Unit> Handle(AnalysisRequest request, CancellationToken cancellationToken)
    {
        var chat = request.Chat;

        var responseBuilder = new StringBuilder()
            .AppendLine(chat.Title)
            .AppendLine()
            .Append("ID: ")
            .AppendLine(chat.Id.ToString())
            .Append("Тип: ")
            .AppendLine(chat.Type.ToString());

        if (chat.Username is not null)
            responseBuilder
                .Append("Ссылка: @")
                .AppendLine(chat.Username);

        if (chat.InviteLink is not null)
            responseBuilder
                .AppendLine("Инвайт: ")
                .AppendLine(chat.InviteLink);

        await _bot.SendTextMessageAsync(request.ChatId, responseBuilder.ToString(),
            cancellationToken: cancellationToken);
        return Unit.Value;
    }
}