using Radzinsky.Bot.Models;

namespace Radzinsky.Bot.Abstractions;

public interface IWebSearchService
{
    public Task<WebSearchResponse> SearchAsync(string query);
}