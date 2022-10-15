using Radzinsky.Application.Abstractions;
using Radzinsky.Domain.Models.Entities;
using Radzinsky.Persistence;

namespace Radzinsky.Application.Services;

public class StateService : IStateService
{
    private readonly ApplicationDbContext _dbContext;

    public StateService(ApplicationDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<T?> ReadStateAsync<T>(string key) where T : State =>
        await FindEntryAsync(key) as T;

    public async Task WriteStateAsync(State state)
    {
        if (await FindEntryAsync(state.Key) is null)
            await _dbContext.States.AddAsync(state);

        await _dbContext.SaveChangesAsync();
    }

    private async Task<State?> FindEntryAsync(string key) =>
        await _dbContext.States.FindAsync(key);
}