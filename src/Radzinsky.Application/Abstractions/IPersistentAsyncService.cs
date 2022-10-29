using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Models.Resources;
using Radzinsky.Application.Services;

namespace Radzinsky.Application.Abstractions;

public interface IPersistentAsyncService
{
    public StateMachineProvider AwaitCallback(string identifier);
    public StateMachineProvider RetrieveState(string identifier);
}