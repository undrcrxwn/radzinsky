using Radzinsky.Domain.Models.Entities;

namespace Radzinsky.Application.Abstractions;

public interface IStateService
{
    public Task<T?> ReadStateAsync<T>(string stateKey) where T : State;
    public Task WriteStateAsync(State state);
}