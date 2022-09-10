using System.Text;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Commands;

public class WebSearchCommand : ICommand
{
    private readonly IWebSearchService _webSearch;
    private readonly IInteractionService _interaction;

    public WebSearchCommand(IWebSearchService webSearch) =>
        _webSearch = webSearch;

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Checkpoint is not null)
            context.ResetCheckpoint();
        else if (string.IsNullOrWhiteSpace(context.Payload))
        {
            await context.ReplyAsync(context.Resources.Variants["SearchWhat"].PickRandom());
            context.SetCommandCheckpoint("SearchWhat");
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
            stringBuilder.Append(context.Resources.Variants["NothingFound"].PickRandom() + " ");

        // Append web search request URL
        stringBuilder.Append($"<a href=\"{url}\">");
        stringBuilder.Append(somethingFound
            ? context.Resources.Variants["WatchAllResults"].PickRandom()
            : context.Resources.Variants["TryYourself"].PickRandom());
        stringBuilder.Append("</a>");

        var text = stringBuilder.ToString();

        // Send reply
        cancellationToken.ThrowIfCancellationRequested();
        await context.ReplyAsync(text, ParseMode.Html, true);
    }
}