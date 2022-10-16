using Radzinsky.Application.Abstractions;
using Radzinsky.Domain.Models.Entities;
using Radzinsky.Persistence;

namespace Radzinsky.Application.Services;

public class StateService : IStateService
{
    private readonly ApplicationDbContext _dbContext;

    public StateService(ApplicationDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<T?> ReadStateAsync<T>(string key) where T : class =>
        (await FindEntryAsync(key))?.Payload as T;

    public async Task WriteStateAsync(string key, object payload)
    {
        var entry = await FindEntryAsync(key);
        
        if (entry is null)
            await _dbContext.States.AddAsync(new State(key, payload));
        else
            entry.Payload = payload;
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task<State?> FindEntryAsync(string key) =>
        await _dbContext.States.FindAsync(key);
}