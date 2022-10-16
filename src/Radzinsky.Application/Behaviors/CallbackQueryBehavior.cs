using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Commands;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Behaviors;

public class CallbackQueryBehavior : IBehavior
{
    private readonly IEnumerable<ICallbackQueryHandler> _callbackQueryHandlers;
    private readonly CallbackQueryContext _callbackQueryContext;
    private readonly IHashingService _hasher;

    public CallbackQueryBehavior(
        IEnumerable<ICallbackQueryHandler> callbackQueryHandlers,
        CallbackQueryContext callbackQueryContext,
        IHashingService hasher)
    {
        _callbackQueryHandlers = callbackQueryHandlers;
        _callbackQueryContext = callbackQueryContext;
        _hasher = hasher;
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
        
        await callbackQueryHandler.HandleCallbackQueryAsync(_callbackQueryContext, new CancellationTokenSource().Token);
    }
}