using System.Text;
using MediatR;
using Radzinsky.Bot.Abstractions;
using Radzinsky.Bot.Attributes;
using Radzinsky.Bot.Enumerations;
using Radzinsky.Bot.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Bot.Imperatives;

public record SearchRequest(long ChatId, string Query) : IRequest;

[ImperativeCallMapping(ImperativeType.Search)]
[ImperativeArgumentParsingStrategy(ImperativeType.Search)]
public class SearchImperative : IImperative<SearchRequest>
{
    private readonly ITelegramBotClient _bot;
    private readonly IWebSearchService _search;

    private readonly IEnumerable<string> _answers = new[]
    {
        "Упс... Что-то я ничего не нашёл.", "Хм... Пустовато...", "Не гуглится.",
        "Не смог ничего найти.", "Я о таком не слышал.", "Изысканные у тебя запросы..."
    };

    public SearchImperative(ITelegramBotClient bot, IWebSearchService search)
    {
        _bot = bot;
        _search = search;
    }

    public IEnumerable<object>? TryParseArguments(ReadOnlyMemory<char> text)
    {
        return new[] {text.ToString()};
    }

    public IBaseRequest MapToRequest(Message context, IEnumerable<object> arguments)
    {
        var queryObject = arguments.SingleOrDefault()
                          ?? throw new ArgumentException(nameof(arguments));

        if (queryObject is string query)
            return new SearchRequest(context.Chat.Id, query);

        throw new ArgumentException(nameof(arguments));
    }

    public async Task<Unit> Handle(SearchRequest request, CancellationToken cancellationToken)
    {
        // Perform web search
        var (results, url) = await _search.SearchAsync(request.Query);
        var somethingFound = results.Any();

        // Append results
        var stringBuilder = new StringBuilder();
        foreach (var result in results)
        {
            stringBuilder.Append($"<a href=\"{result.Url}\">");
            stringBuilder.Append(result.Title);
            stringBuilder.AppendLine("</a>");
            stringBuilder.AppendLine(result.Description);
            stringBuilder.AppendLine();
        }

        // Append "nothing found" answer
        if (!somethingFound)
            stringBuilder.Append((string?) _answers.PickRandom());

        // Append web search request URL
        stringBuilder.Append($"<a href=\"{url}\">");
        stringBuilder.Append(somethingFound
            ? "💬 Просмотреть все результаты"
            : " 👉 Поискать самостоятельно");
        stringBuilder.Append("</a>");

        var text = stringBuilder.ToString();

        // Send reply
        await _bot.SendTextMessageAsync(request.ChatId, text, ParseMode.Html, disableWebPagePreview: true,
            cancellationToken: cancellationToken);
        return Unit.Value;
    }
}