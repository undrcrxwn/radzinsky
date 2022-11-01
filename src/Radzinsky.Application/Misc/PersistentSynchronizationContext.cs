using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Radzinsky.Application.Abstractions;
using Serilog;

namespace Radzinsky.Application.Misc;

public sealed class PersistentSynchronizationContext : SynchronizationContext
{
    private const string StateMachinePropertyName = "StateMachine";

    public string? Identifier;
    public bool IsStateRetrieved;
    public IAsyncStateMachine? Machine { get; private set; }

    public override void Post(SendOrPostCallback callback, object? state)
    {
        try
        {
            Machine ??= (IAsyncStateMachine)state?.GetType()
                .GetField(StateMachinePropertyName)!
                .GetValue(state)!;
        }
        catch
        {
        }

        base.Post(s =>
        {
            SetSynchronizationContext(this);
            callback(s);
        }, state);
    }
}