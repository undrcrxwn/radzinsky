using System.Diagnostics.CodeAnalysis;
using System.Text;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Commands;

public class WebSearchCommand : ICommand
{
    private readonly IWebSearchService _webSearch;

    public WebSearchCommand(IWebSearchService webSearch) =>
        _webSearch = webSearch;

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.Payload))
        {
            await context.SendTextAsync(context.Resources!.GetRandom<string>("SearchWhat"));
            context.SetCheckpoint("SearchWhat");
            return;
        }
        
        // Perform web search
        var (results, url) = await _webSearch.SearchAsync(context.Payload);
        cancellationToken.ThrowIfCancellationRequested();
        
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
        var somethingFound = results.Any();
        if (!somethingFound)
            stringBuilder.Append(context.Resources!.GetRandom<string>("NothingFound") + " ");

        // Append web search request URL
        stringBuilder.Append($"<a href=\"{url}\">");
        stringBuilder.Append(somethingFound
            ? context.Resources!.GetRandom<string>("WatchAllResults")
            : context.Resources!.GetRandom<string>("TryYourself"));
        stringBuilder.Append("</a>");

        var text = stringBuilder.ToString();

        // Send reply
        cancellationToken.ThrowIfCancellationRequested();
        await context.SendTextAsync(text, ParseMode.Html, disableWebPagePreview: true);
    }
}