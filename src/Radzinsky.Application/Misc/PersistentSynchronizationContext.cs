using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Radzinsky.Application.Misc;

public sealed class PersistentSynchronizationContext : SynchronizationContext
{
    private const string StateMachinePropertyName = "StateMachine";
    private const string StateMachineStateFieldName = "<>1__state";
    private const string StateMachineBuilderFieldName = "<>t__builder";
    public object? TargetState { get; private set; }
    public object? TargetStateMachine { get; private set; }

    private static readonly IEnumerable<string> IgnoredStateMachineFieldNames = new[]
    {
        StateMachineStateFieldName,
        StateMachineBuilderFieldName
    };

    public override void Post(SendOrPostCallback action, object? state)
    {;
        object? stateMachine = null;

        try
        {
            stateMachine = GetStateMachineFromState(state!);
            TargetStateMachine ??= (stateMachine is null ? null : stateMachine);
            TargetState ??= state;
        }
        catch
        {
            Log.Information("Failed to extract state machine object from received state");
        }
        
        Log.Warning("POST: TARGET  STATE: {0}", TargetState?.GetType().Name);
        Log.Warning("POST: TARGET    ASM: {0}", TargetStateMachine?.GetType().Name);
        Log.Warning("POST: CURRENT STATE: {0}", state?.GetType().Name);
        Log.Warning("POST: CURRENT   ASM: {0}", stateMachine?.GetType().Name);

        base.Post(x => InvokeDecoratedAction(action, x), state);
    }

    private void InvokeDecoratedAction(SendOrPostCallback action, object? state)
    {
        SetSynchronizationContext(this);
        
        if (state != TargetState)
        {
            // Not performing anything special if the action is not posted from the original targeted async method
            action(state);
            return;
        }

        Log.Warning("POSTING PERSISTENT FOR " + state.GetType().Name);

        //LoadStateMachineData(stateMachine);
        var before = (int)TargetStateMachine.GetType().GetField(StateMachineStateFieldName, BindingFlags.Instance | BindingFlags.Public)!
            .GetValue(TargetStateMachine)!;

        var now = (int)TargetStateMachine.GetType().GetField(StateMachineStateFieldName, BindingFlags.Instance | BindingFlags.Public)!
            .GetValue(TargetStateMachine)!;
        Console.WriteLine($"Running action in state {now}");
        
        action(state);
    }

    private static TValue GetStateMachineField<TValue>(object stateMachine, string fieldName) =>
        (TValue)stateMachine.GetType()
            .GetField(fieldName, BindingFlags.Instance | BindingFlags.Public)!
            .GetValue(stateMachine)!;
    
    private static object GetStateMachineFromState(object state)
    {
        var stateType = state.GetType();
        var stateMachineField = stateType.GetField(StateMachinePropertyName, BindingFlags.Instance | BindingFlags.Public)!;
        return stateMachineField.GetValue(state)!;
    }

    private static void LoadStateMachineData(object stateMachine)
    {
        /*var stateMachineType = stateMachine.GetType();

        var data = JObject.Parse(json);

        var stateMachineFields = stateMachineType.GetFields(BindingFlags.Instance | BindingFlags.Public)
            .ExceptBy(IgnoredStateMachineFieldNames, x => x.Name)
            .ToList();

        stateMachineFields.ForEach(x => x.SetValue(stateMachine, data[x.Name]!.ToObject(x.FieldType)));*/
    }
}