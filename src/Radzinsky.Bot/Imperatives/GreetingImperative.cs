using MediatR;
using Radzinsky.Bot.Abstractions;
using Radzinsky.Bot.Attributes;
using Radzinsky.Bot.Enumerations;
using Radzinsky.Bot.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Bot.Imperatives;

public record GreetingRequest(long ChatId, string Name) : IRequest;

[ImperativeCallMapping(ImperativeType.Greeting)]
[ImperativeArgumentParsingStrategy(ImperativeType.Greeting)]
public class GreetingImperative : IImperative<GreetingRequest>
{
    private readonly ITelegramBotClient _bot;

    public GreetingImperative(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public IEnumerable<object>? TryParseArguments(ReadOnlyMemory<char> text)
    {
        return Enumerable.Empty<object>();
    }

    public IBaseRequest MapToRequest(Message context, IEnumerable<object> arguments)
    {
        return new GreetingRequest(context.Chat.Id, context.From.FirstName);
    }

    public async Task<Unit> Handle(GreetingRequest request, CancellationToken cancellationToken)
    {
        var answers = new[]
        {
            "Ну привет", "Привет", "Привет.", "Давно не виделись", "Как оно?", "Ку", "👋",
            "Доброго времени", "Чего тебе?", "Привет! Где пропадал?", $"Чего тебе, {request.Name}?",
            $"Привет, {request.Name}.", $"Здравствуй, {request.Name}."
        };

        await _bot.SendTextMessageAsync(request.ChatId, answers.PickRandom(), ParseMode.Html,
            cancellationToken: cancellationToken);
        return Unit.Value;
    }
}