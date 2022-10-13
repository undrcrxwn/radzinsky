using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Behaviors;

public class CallbackQueryBehavior : IBehavior
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEnumerable<ICallbackQueryHandler> _callbackQueryHandlers;
    private readonly CallbackQueryContext _callbackQueryContext;

    public CallbackQueryBehavior(
        IServiceScopeFactory scopeFactory,
        IEnumerable<ICallbackQueryHandler> callbackQueryHandlers,
        CallbackQueryContext callbackQueryContext)
    {
        _scopeFactory = scopeFactory;
        _callbackQueryHandlers = callbackQueryHandlers;
        _callbackQueryContext = callbackQueryContext;
    }

    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        if (context.Update.Type != UpdateType.CallbackQuery)
        {
            await next(context);
            return;
        }

        _callbackQueryContext.Query = context.Update.CallbackQuery!;

        using var scope = _scopeFactory.CreateScope();
        var callbackQueryHandler = _callbackQueryHandlers.First(x => x.GetType().FullName == _callbackQueryContext.HandlerTypeName);
        await callbackQueryHandler.HandleCallbackQueryAsync(_callbackQueryContext, new CancellationTokenSource().Token);
    }
}