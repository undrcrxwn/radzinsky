using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Misc;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Models.Resources;
using Serilog;

namespace Radzinsky.Application.Services;

public class PersistentAsyncService : IPersistentAsyncService
{
    private const string StateMachineStateFieldName = "<>1__state";
    private const string StateMachineBuilderFieldName = "<>t__builder";

    private static readonly IEnumerable<string> IgnoredStateMachineFieldNames = new[]
    {
        StateMachineStateFieldName,
        StateMachineBuilderFieldName
    };

    private readonly IStateService _states;

    public PersistentAsyncService(IStateService states) =>
        _states = states;

    public StateMachineProvider AwaitCallback(string identifier) => new(stateMachine =>
    {
        Log.Warning($"AwaitCallback {identifier}");

        var stateMachineType = stateMachine.GetType();
        var stateMachineFields = stateMachineType
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => !x.Name.StartsWith('<') || x.Name == StateMachineStateFieldName)
            .ToDictionary(x => x.Name, x =>
            {
                string? serializedValue;

                try
                {
                    serializedValue = JsonConvert.SerializeObject(x.GetValue(stateMachine));
                }
                catch
                {
                    serializedValue = null;
                }

                return new FieldData(
                    x.FieldType.FullName!,
                    serializedValue);
            });

        var stateKey = GetStateKey(stateMachineType.FullName!, identifier);
        _states.WriteStateAsync(stateKey, stateMachineFields).Wait();
        Log.Information("Async state machine state has been saved");

        throw new AsyncOperationInterruptedException();
    });

    public StateMachineProvider RetrieveState(string identifier) => new(async stateMachine =>
    {
        Log.Warning($"RetrieveState {identifier}");

        var stateMachineType = stateMachine.GetType();
        var stateKey = GetStateKey(stateMachineType.FullName!, identifier);
        var fieldMap = await _states.ReadStateAsync<IDictionary<string, FieldData>>(stateKey);

        if (fieldMap is null)
            return;

        var stateMachineFields = stateMachineType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => !x.Name.StartsWith('<') || x.Name == StateMachineStateFieldName)
            .ToList();

        stateMachineFields.ForEach(x =>
        {
            var fieldData = fieldMap[x.Name];

            Log.Warning("Retreiving field {0} of type {1} with value {2}",
                x.Name, fieldData.TypeName, fieldData.SerializedValue);
            try
            {
                x.SetValue(
                    stateMachine,
                    JsonConvert.DeserializeObject(
                        fieldData.SerializedValue,
                        Type.GetType(fieldData.TypeName)!));
            }
            catch
            {
                Log.Warning("Failed to retreive field {0} of type {1}",
                    x.Name, fieldData.TypeName);
            }
        });

        var stateField = stateMachineFields.Find(x => x.Name == StateMachineStateFieldName)!;
        var state = (int)stateField.GetValue(stateMachine)!;
        stateField.SetValue(stateMachine, state + 1);
    });

    private static string GetStateKey(string stateMachineTypeName, string? identifier) =>
        $"{stateMachineTypeName} {identifier ?? string.Empty}";

    private record FieldData(string TypeName, string SerializedValue);
}

public class StateMachineProvider
{
    private readonly Awaiter _awaiter;

    public StateMachineProvider(Func<IAsyncStateMachine, Task> handler) =>
        _awaiter = new Awaiter(handler);

    public Awaiter GetAwaiter() => _awaiter;

    public class Awaiter : INotifyCompletion
    {
        private const string StateMachineFieldName = "StateMachine";
        private readonly Func<IAsyncStateMachine, Task> _handler;

        private Action? _continuation;

        public Awaiter(Func<IAsyncStateMachine, Task> handler) =>
            _handler = handler;

        public bool IsCompleted => _continuation is not null;

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
            Task.Run(continuation);
        }

        public void GetResult()
        {
            var target = _continuation!.Target!;
            var field = target.GetType()
                .GetField(StateMachineFieldName, BindingFlags.Instance | BindingFlags.Public)!;
            var stateMachine = (IAsyncStateMachine)field.GetValue(target)!;
            _handler(stateMachine).Wait();
        }
    }
}