using System.Reflection;
using System.Runtime.CompilerServices;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Misc;
using Radzinsky.Application.Misc.Attributes;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Models.Resources;
using Radzinsky.Application.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace Radzinsky.Application.Commands;

public class VoteKickCommand : ICommand, ICallbackQueryHandler
{
    private readonly IPersistentAsyncService _async;

    public VoteKickCommand(IPersistentAsyncService async)
    {
        _async = async;
    }
    
    [PersistedAsyncState]
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        await _async.RetrieveCurrentState(context.Update.ChatId.ToString()!);

        await context.SendTextAsync("1");
        await _async.AwaitCallback();
        await context.SendTextAsync("2");
        await _async.AwaitCallback();
        await context.SendTextAsync("3");
        await _async.AwaitCallback();
        await context.SendTextAsync("4");
        await _async.AwaitCallback();
        await context.SendTextAsync("5");
    }

    public async Task HandleCallbackQueryAsync(CallbackQueryContext context, CancellationToken token)
    {
        await Task.Yield();
        await context.SendTextAsync($"Callback query received with data: {context.Query.Data}");
    }
}