using System.Reflection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Misc;
using Radzinsky.Application.Misc.Attributes;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Behaviors;

public class CallbackQueryBehavior : IBehavior
{
    private readonly IEnumerable<ICallbackQueryHandler> _callbackQueryHandlers;
    private readonly CallbackQueryContext _callbackQueryContext;
    private readonly IHashingService _hasher;
    private readonly IStateService _states;

    public CallbackQueryBehavior(
        IEnumerable<ICallbackQueryHandler> callbackQueryHandlers,
        CallbackQueryContext callbackQueryContext,
        IHashingService hasher,
        IStateService states)
    {
        _callbackQueryHandlers = callbackQueryHandlers;
        _callbackQueryContext = callbackQueryContext;
        _hasher = hasher;
        _states = states;
    }

    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        if (context.Update.Type != UpdateType.CallbackQuery)
        {
            await next(context);
            return;
        }

        _callbackQueryContext.Update = context.Update;
        _callbackQueryContext.Query = context.Update.CallbackQuery!;

        var callbackQueryHandler = _callbackQueryHandlers.First(x =>
            _hasher.HashKey(x.GetType().FullName!) == _callbackQueryContext.Query.CallbackHandlerTypeNameHash);
        
        var shouldRunWithPersistedState =
            callbackQueryHandler.GetType().GetMethod(nameof(ICommand.ExecuteAsync))!
                .GetCustomAttribute<PersistedAsyncStateAttribute>() is not null;

        if (shouldRunWithPersistedState)
        {
            var synchronizationContext = new PersistentSynchronizationContext(_states);
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
        }

        try
        {
            await callbackQueryHandler.HandleCallbackQueryAsync(_callbackQueryContext, new CancellationTokenSource().Token);
        }
        catch (AsyncOperationInterruptedException) {}
    }
}