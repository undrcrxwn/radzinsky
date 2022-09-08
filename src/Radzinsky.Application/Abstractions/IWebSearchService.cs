using Radzinsky.Application.Models;

namespace Radzinsky.Application.Abstractions;

public interface IWebSearchService
{
    public Task<WebSearchResponse> SearchAsync(string query);
}