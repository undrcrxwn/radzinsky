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
        
        var xField = stateMachineFields.Find(x => x.Name == "<x>5__2")!;
        xField.SetValue(stateMachine, 1337);
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
            var c = _continuation ?? new Action(() => {
            });
            
            var machine = Executioner.GetStateMachine(c);
            _handler(machine).Wait();
            /*var target = _continuation!.Target!;
            var field = target.GetType()
                .GetField(StateMachineFieldName, BindingFlags.Instance | BindingFlags.Public)!;
            var stateMachine = (IAsyncStateMachine)field.GetValue(target)!;
            await _handler(stateMachine);*/
        }
    }
}






interface ITransient
{
}

public class Executioner : ITransient
    {
        private readonly Action<Executioner> entryPoint;
        private Action nextAction;

        public Executioner(Action<Executioner> entryPoint)
        {
            this.entryPoint = entryPoint;
        }

        public void Start()
        {
            Execute(entryPoint);
        }

        public static IAsyncStateMachine GetStateMachine(Action continuation)
        {
            var target = continuation.Target;
            var field = target.GetType().GetField("StateMachine", BindingFlags.Public | BindingFlags.Instance);
            return (IAsyncStateMachine) field.GetValue(target);
        }
        
        protected void Execute(Action<Executioner> action)
        {       
            nextAction = () => action(this);
            while (nextAction != null)
            {
                Action next = nextAction;
                nextAction = null;
                next();
            }
        }

        public IAwaitable CreateAwaitable(Action<IAsyncStateMachine> stateMachineHandler)
        {
            var awaiter = new YieldingAwaiter(continuation =>
            {
                var machine = GetStateMachine(continuation);
                stateMachineHandler(machine);
                nextAction = continuation;
            });
            return awaiter.NewAwaitable();
        }

        public IAwaitable CreateAwaitable(Func<IAsyncStateMachine, Action> stateMachineHandler)
        {
            var awaiter = new YieldingAwaiter(continuation =>
            {
                var machine = GetStateMachine(continuation);
                nextAction = stateMachineHandler(machine);
            });
            return awaiter.NewAwaitable();
        }

        public YieldingAwaitable<T> CreateYieldingAwaitable<T>(Action<IAsyncStateMachine> stateMachineHandler, T result)
        {
            var awaiter = new YieldingAwaiter<T>(continuation =>
            {
                var machine = GetStateMachine(continuation);
                stateMachineHandler(machine);
                nextAction = continuation;
            }, result);
            return new YieldingAwaitable<T>(awaiter);
        }

        public YieldingAwaitable<T> CreateYieldingAwaitable<T>(Func<IAsyncStateMachine, Action> stateMachineHandler, T value)
        {
            var awaiter = new YieldingAwaiter<T>(continuation =>
            {
                var machine = GetStateMachine(continuation);
                nextAction = stateMachineHandler(machine);
            }, value);
            return new YieldingAwaitable<T>(awaiter);
        }
    }

public struct YieldingAwaiter : IAwaiter
{
    private readonly Action<Action> onCompletedHandler;

    public YieldingAwaiter(Action<Action> onCompletedHandler)
    {
        this.onCompletedHandler = onCompletedHandler;
    }

    public bool IsCompleted { get { return false; } }

    public void GetResult()
    {            
    }

    public void OnCompleted(Action continuation)
    {
        onCompletedHandler(continuation);
    }
}

public class YieldingAwaitable<T>
{
    private readonly YieldingAwaiter<T> awaiter;

    internal YieldingAwaitable(YieldingAwaiter<T> awaiter)
    {
        this.awaiter = awaiter;
    }

    public YieldingAwaiter<T> GetAwaiter()
    {
        return awaiter;
    }
}

public struct YieldingAwaiter<T> : IAwaiter<T>
{
    private readonly Action<Action> onCompletedHandler;
    private readonly T result;

    public YieldingAwaiter(Action<Action> onCompletedHandler, T result)
    {
        this.onCompletedHandler = onCompletedHandler;
        this.result = result;
    }

    public bool IsCompleted { get { return false; } }

    public T GetResult()
    {
        return result;
    }

    public void OnCompleted(Action continuation)
    {
        onCompletedHandler(continuation);
    }
}
    
public interface IAwaitable
{
    IAwaiter GetAwaiter();
}

public interface IAwaitable<T>
{
    IAwaiter<T> GetAwaiter();
}
    
public interface IAwaiter : INotifyCompletion
{
    bool IsCompleted { get; }
    void GetResult();
}

public interface IAwaiter<out T> : INotifyCompletion
{
    bool IsCompleted { get; }
    T GetResult();
}

public static class AwaiterExtensions
{
    private class Awaitable : IAwaitable
    {
        private readonly IAwaiter awaiter;

        internal Awaitable(IAwaiter awaiter)
        {
            this.awaiter = awaiter;
        }

        public IAwaiter GetAwaiter()
        {
            return awaiter;
        }
    }

    public static IAwaitable NewAwaitable(this IAwaiter awaiter)
    {
        return new Awaitable(awaiter);
    }

    private class Awaitable<T> : IAwaitable<T>
    {
        private readonly IAwaiter<T> awaiter;

        internal Awaitable(IAwaiter<T> awaiter)
        {
            this.awaiter = awaiter;
        }

        public IAwaiter<T> GetAwaiter()
        {
            return awaiter;
        }
    }

    public static IAwaitable<T> NewAwaitable<T>(this IAwaiter<T> awaiter)
    {
        return new Awaitable<T>(awaiter);
    }
}