using System.Runtime.CompilerServices;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Models.Resources;
using Radzinsky.Application.Services;

namespace Radzinsky.Application.Abstractions;

public interface IPersistentAsyncService
{
    public Task AwaitCallback();
    public YieldAwaitable RetrieveState(string identifier);
}