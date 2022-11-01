using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Misc;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Models.Resources;
using Serilog;

namespace Radzinsky.Application.Services;

public class PersistentAsyncService : IPersistentAsyncService
{
    private record FieldData(string TypeName, string SerializedValue);

    private const string StateMachineStateFieldName = "<>1__state";
    private const string StateMachineBuilderFieldName = "<>t__builder";

    private static readonly IEnumerable<string> IgnoredStateMachineFieldNames = new[]
    {
        StateMachineBuilderFieldName
    };

    private readonly IStateService _states;

    public PersistentAsyncService(IStateService states) =>
        _states = states;

    public YieldAwaitable SaveCurrentState(string? identifier = null)
    {
        var context = (PersistentSynchronizationContext)SynchronizationContext.Current!;
        
        context.ScheduleForNextAwait(machine =>
        {
            Log.Information("Job `SaveCurrentState` starting");
            
            if (machine is null)
                return;
            
            Log.Information("Job `SaveCurrentState` in progress");
            
            SaveState(machine, identifier).Wait();
        });
        
        return Task.Yield();
    }
    
    public async Task SaveState(IAsyncStateMachine machine, string? identifier = null)
    {
        var machineType = machine.GetType();
        var machineFields = machineType
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => !x.Name.StartsWith('<') || x.Name == StateMachineStateFieldName)
            .ToDictionary(x => x.Name, x =>
            {
                string? serializedValue;

                try
                {
                    serializedValue = JsonConvert.SerializeObject(x.GetValue(machine));
                }
                catch
                {
                    serializedValue = JsonConvert.SerializeObject(null);
                }

                return new FieldData(
                    x.FieldType.FullName!,
                    serializedValue);
            });

        var stateKey = GetStateKey(machineType.FullName!, identifier);
        await _states.WriteStateAsync(stateKey, machineFields);
        
        Log.Warning("Async state saved by key {0}", stateKey);
    }

    public YieldAwaitable RetrieveCurrentState(string? identifier = null)
    {
        var context = (PersistentSynchronizationContext)SynchronizationContext.Current!;
        
        context.ScheduleForNextAwait(machine =>
        {
            Log.Information("Job `RetrieveCurrentState` starting");
            
            if (context.IsStateRetrieved || machine is null)
                return;
            
            Log.Information("Job `RetrieveCurrentState` in progress");
                
            RetrieveState(machine, identifier).Wait();
            context.IsStateRetrieved = true;
        });
        
        return Task.Yield();
    }
    
    public async Task RetrieveState(IAsyncStateMachine machine, string? identifier = null)
    {
        var machineType = machine.GetType();
        var stateKey = GetStateKey(machineType.FullName!, identifier);
        var valueMap = await _states.ReadStateAsync<IDictionary<string, FieldData>>(stateKey);

        if (valueMap is null)
            return;

        var stateMachineFields = machineType.GetFields(BindingFlags.Instance | BindingFlags.Public)
            .ExceptBy(IgnoredStateMachineFieldNames, x => x.Name)
            .ToList();

        stateMachineFields.ForEach(x =>
        {
            var serializedValue = valueMap[x.Name].SerializedValue;
            var deserializedValue = JsonConvert.DeserializeObject(serializedValue, x.FieldType);

            var value =
                x.Name == StateMachineStateFieldName
                    ? (int)deserializedValue! + 1
                    : deserializedValue;

            x.SetValue(machine, value);
        });
        
        Log.Warning("Async state retrieved by key {0}", stateKey);
    }

    public async Task AwaitCallback()
    {
        await SaveCurrentState();
        Log.Warning("Interrupting...");
        throw new AsyncOperationInterruptedException();
    }
    
    private static string GetStateKey(string machineTypeName, string? identifier) =>
        identifier is not null
            ? $"{machineTypeName}__{identifier}"
            : machineTypeName;
}


#if false
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
            var c = _continuation ?? new Action(() => { });

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
        return (IAsyncStateMachine)field.GetValue(target);
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

    public bool IsCompleted
    {
        get { return false; }
    }

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

    public bool IsCompleted
    {
        get { return false; }
    }

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

#endif