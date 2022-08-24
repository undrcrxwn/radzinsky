namespace Radzinsky.Bot.Models;

public record WebSearchResponse(IEnumerable<WebSearchResult> Results, string SearchUrl);