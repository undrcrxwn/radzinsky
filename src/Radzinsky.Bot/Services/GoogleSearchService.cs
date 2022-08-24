using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.CustomSearchAPI.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Radzinsky.Bot.Abstractions;
using Radzinsky.Bot.Models;

namespace Radzinsky.Bot.Services;

public class GoogleSearchService : IWebSearchService
{
    private const string GoogleSearchApiTokenVariableKey = "GOOGLE_SEARCH_API_KEY";

    private readonly CustomSearchAPIService _searchClient;
    private readonly IConfiguration _configuration;

    public GoogleSearchService(IConfiguration configuration)
    {
        _configuration = configuration;

        var initializer = new BaseClientService.Initializer
        {
            ApiKey = Environment.GetEnvironmentVariable(GoogleSearchApiTokenVariableKey)
        };

        _searchClient = new CustomSearchAPIService(initializer);
    }

    public async Task<WebSearchResponse> SearchAsync(string query)
    {
        var url = $"https://www.google.com/search?q={query}";
        var response = await FormRequest(query).ExecuteAsync();

        var results = (response.Items ?? Enumerable.Empty<Result>())
            .Select(item => new WebSearchResult(item.Title, item.Snippet, item.Link));

        return new WebSearchResponse(results, url);
    }

    private CseResource.ListRequest FormRequest(string query)
    {
        var request = _searchClient.Cse.List();
        request.Cx = _configuration["GoogleSearch:SearchEngineId"];
        request.Q = query;
        request.Gl = _configuration["GoogleSearch:SearchEngineId"];
        request.Num = _configuration.GetValue<int>("GoogleSearch:MaxResultCount");

        return request;
    }
}