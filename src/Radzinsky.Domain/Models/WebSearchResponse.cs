namespace Radzinsky.Domain.Models;

public record WebSearchResponse(IEnumerable<WebSearchResult> Results, string SearchUrl);