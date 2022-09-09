using Microsoft.AspNetCore.Mvc;
using Radzinsky.Application.Abstractions;
using Serilog;
using Telegram.Bot.Types;

namespace Radzinsky.Host.Controllers;

public class WebhookController : Controller
{
    private static readonly TimeSpan MaxUpdateTimeout = TimeSpan.FromSeconds(5);
    
    public async Task<IActionResult> Post(
        [FromServices] IUpdateHandler updateHandler,
        [FromBody] Update update)
    {
        if (update.Message is null)
            return Ok("Update has no message");
        
        if (DateTime.UtcNow - update.Message.Date.ToUniversalTime() > MaxUpdateTimeout)
            return Ok("Update timed out");
        
        Log.Information("Incoming message update from {0} at chat {1} ({2})",
            update.Message.From, update.Message.Chat.Username, update.Message.Chat.Id);

        var cts = new CancellationTokenSource();
        await updateHandler.HandleAsync(update, cts.Token);
        cts.Token.ThrowIfCancellationRequested();
        
        return Ok();
    }
}