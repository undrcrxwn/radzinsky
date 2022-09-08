namespace Radzinsky.Application.Models;

public record WebSearchResponse(IEnumerable<WebSearchResult> Results, string SearchUrl);