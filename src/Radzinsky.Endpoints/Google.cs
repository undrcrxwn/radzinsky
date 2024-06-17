using System.Text;
using System.Web;
using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using Radzinsky.Framework;
using Radzinsky.Framework.Exceptions;
using Radzinsky.Framework.Routing;
using Radzinsky.Framework.Routing.RegEx;
using Radzinsky.Framework.Routing.StringDistance;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Endpoints;

[RegExPatterns(
    @"(?i)^\/google(?:@radzinsky_bot)? ?(.*)$",
    @"(?i)^\/search(?:@radzinsky_bot)? ?(.*)$",
    @"(?i)^\/гугл(?:@radzinsky_bot)? ?(.*)$")]
[StringDistanceAliases(
    "гугл", "загугли", "найди", "что такое", "что значит", "что означает",
    "объясни", "объясни что такое", "объясни что значит", "объясни что означает",
    "расскажи что такое", "расскажи что значит", "расскажи что означает", "расскажи про")]
public class Google(ITelegramBotClient bot, IOptions<Google.Configuration> configuration) : IEndpoint
{
    public class Configuration
    {
        public string Token { get; set; } = null!;
        public string Cx { get; set; } = null!;
    }

    public async Task HandleAsync(Update update, Route route, CancellationToken cancellationToken)
    {
        var query = route switch
        {
            RegExRoute regExRoute => regExRoute.Groups[1].Value,
            StringDistanceRoute stringDistanceRoute => stringDistanceRoute.TailSegment.ToString(),
            _ => throw new UnsupportedRouteException()
        };

        if (string.IsNullOrWhiteSpace(query))
        {
            await bot.SendTextMessageAsync(
                chatId: update.Message!.Chat,
                text: $"Укажи поисковой запрос. Например вот так: «{update.Message.Text} механизация крыла».",
                cancellationToken: cancellationToken);
            return;
        }

        var response = await GoogleAsync(query);
        await bot.SendTextMessageAsync(
            chatId: update.Message!.Chat,
            text: response,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);
    }

    private async Task<string> GoogleAsync(string query)
    {
        var urlEncodedQuery = HttpUtility.UrlEncode(query);
        var url = $"https://www.google.com/search?q={urlEncodedQuery}";
        var response = await CreateRequest(query).ExecuteAsync();

        var results = (response.Items ?? [])
            .Select(item => (item.Title, item.Snippet, item.Link))
            .ToList();

        var stringBuilder = new StringBuilder();
        foreach (var result in results)
        {
            stringBuilder.Append($"<a href=\"{result.Link}\">");
            stringBuilder.Append(result.Title);
            stringBuilder.AppendLine("</a>");
            stringBuilder.AppendLine(result.Snippet);
            stringBuilder.AppendLine();
        }

        var somethingFound = results.Count > 0;
        if (!somethingFound)
            stringBuilder.Append($"По запросу «{query}» ничего не найдено. ");

        stringBuilder.Append($"<a href=\"{url}\">");
        stringBuilder.Append(somethingFound
            ? "\uD83D\uDCAC Просмотреть все результаты"
            : "\uD83D\uDC49 Загуглить самостоятельно");

        stringBuilder.Append("</a>");
        return stringBuilder.ToString();
    }

    private CseResource.ListRequest CreateRequest(string query)
    {
        var searchClient = new CustomSearchAPIService(new BaseClientService.Initializer
        {
            ApiKey = configuration.Value.Token
        });

        var request = searchClient.Cse.List();
        request.Cx = configuration.Value.Cx;
        request.Q = query;
        request.Gl = "ru";
        request.Num = 3;

        return request;
    }
}