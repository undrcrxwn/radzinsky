using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Radzinsky.Application.Abstractions;
using Serilog;

namespace Radzinsky.Application.Misc;

public sealed class PersistentSynchronizationContext : SynchronizationContext
{
    public string? Identifier;
    public bool IsStateRetrieved;
    public IAsyncStateMachine? Machine { get; private set; }

    private record FieldData(string TypeName, string SerializedValue);


    private const string StateMachinePropertyName = "StateMachine";
    private const string StateMachineStateFieldName = "<>1__state";
    private const string StateMachineBuilderFieldName = "<>t__builder";

    private static readonly IEnumerable<string> IgnoredStateMachineFieldNames = new[]
    {
        StateMachineBuilderFieldName
    };


    private readonly IStateService _states;
    private Queue<Action<IAsyncStateMachine?>> _scheduledJobs = new();

    public PersistentSynchronizationContext(IStateService states)
    {
        _states = states;
    }

    public override void Post(SendOrPostCallback callback, object? state)
    {
        if (!IsStateRetrieved)
        {
            Machine = (IAsyncStateMachine)state!.GetType()
                .GetField(StateMachinePropertyName)!
                .GetValue(state)!;

            var field = Machine.GetType()
                .GetField("<>1__state", BindingFlags.Instance | BindingFlags.Public)!;
            field.SetValue(Machine, 4);

            IsStateRetrieved = true;
        }
        
        base.Post(s =>
        {
            SetSynchronizationContext(this);
            callback(s);
        }, state);
    }

    public void ScheduleForNextAwait(Action<IAsyncStateMachine?> job) =>
        _scheduledJobs.Enqueue(job);
}