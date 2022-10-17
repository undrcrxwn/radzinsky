using Microsoft.Extensions.Caching.Memory;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Commands;

public class NewsCommand : ICommand
{
    private readonly INewsService _news;
    private readonly IMemoryCache _cache;

    public NewsCommand(INewsService news, IMemoryCache cache)
    {
        _news = news;
        _cache = cache;
    }

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var response = await _cache.GetOrCreateAsync("TodayNews", async x =>
        {
            x.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return await GetWebScrappedResponseAsync();
        });

        await context.DeletePreviousReplyAsync();
        await context.SendTextAsync(response);
    }
    
    private async Task<string> GetWebScrappedResponseAsync()
    {
        var titles = await _news.GetTitlesAsync(DateTime.Today);
        return string.Join("\n\n", titles.Select(title => $"— {title}"));
    }
}