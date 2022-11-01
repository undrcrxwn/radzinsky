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
    private readonly IHashingService _hasher;
    private readonly IPersistentAsyncService _async;

    public VoteKickCommand(IHashingService hasher, IPersistentAsyncService async)
    {
        _hasher = hasher;
        _async = async;
    }
    
    [PersistedAsyncState]
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        await context.SendTextAsync(SynchronizationContext.Current?.GetType().Name ?? "NULL");
        await _async.RetrieveState(context.Update.ChatId.ToString()!);

        var x = 42;
        await context.SendTextAsync($"Initially, the x is {x}.");

        await context.SendTextAsync("We're interrupting now...");
        await _async.AwaitCallback();
        
        await context.SendTextAsync($"As for now, the x is still {x}.");
        await context.SendTextAsync("Bye!");
        
        return;
        
        //await context.SendTextAsync("ASM before Task.Yield: " + SynchronizationContext.Current?.GetType().Name);
        await Task.Yield();
        await context.SendTextAsync("ASM after Task.Yield: " + SynchronizationContext.Current?.GetType().Name);

        var factory = new ButtonFactory<VoteKickCommand>(_hasher, $"{context.Update.InteractorUserId} {{0}}");

        var keyboard = new[]
        {
            new[]
            {
                factory.CreateCallbackDataButton("Yes", "yes"),
                factory.CreateCallbackDataButton("No", "no")
            }
        };

        await context.SendTextAsync("Should we kick the guy? You decide.",
            replyMarkup: new InlineKeyboardMarkup(keyboard));

        await context.SendTextAsync("* I/O interruption *");

        await context.SendTextAsync("ASM before _async.AwaitCallback: " + SynchronizationContext.Current?.GetType().Name);
        //await _async.AwaitCallback(context.Update.ChatId.ToString()!);

        await context.SendTextAsync("* handling I/O result *");
    }

    public async Task HandleCallbackQueryAsync(CallbackQueryContext context, CancellationToken token)
    {
        await Task.Yield();
        await context.SendTextAsync($"Callback query received with data: {context.Query.Data}");
    }
}