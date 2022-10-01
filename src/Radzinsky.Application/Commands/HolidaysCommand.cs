using Microsoft.Extensions.Caching.Memory;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Commands;

public class HolidaysCommand : ICommand
{
    private readonly IHolidaysService _holidays;
    private readonly IMemoryCache _cache;
    
    public HolidaysCommand(IHolidaysService holidays, IMemoryCache cache)
    {
        _holidays = holidays;
        _cache = cache;
    }

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var response = await _cache.GetOrCreateAsync("TodayHolidays", async x =>
        {
            x.AbsoluteExpiration = DateTime.Today.AddDays(1);
            return await GetWebScrappedResponseAsync();
        });

        await context.DeletePreviousReplyAsync();
        await context.ReplyAsync(response);
    }

    private async Task<string> GetWebScrappedResponseAsync()
    {
        var holidays = await _holidays.GetHolidaysAsync();
        return string.Join('\n', holidays.Select(holiday => $"— {holiday}"));
    }
}