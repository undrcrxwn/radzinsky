using Microsoft.AspNetCore.Mvc;
using Radzinsky.Application.Abstractions;
using Serilog;
using Telegram.Bot.Types;

namespace Radzinsky.Host.Controllers;

[ApiController]
public class WebhookController : Controller
{
    public async Task<IActionResult> Post(
        [FromServices] IUpdateHandler updateHandler,
        [FromBody] Update update)
    {
        if (update.Message is null)
            return Ok("Update has no message");
        
        Log.Information("Incoming message update from {0} at chat {1} ({2})",
            update.Message.From, update.Message.Chat.Username, update.Message.Chat.Id);

        var cts = new CancellationTokenSource();
        await updateHandler.HandleAsync(update, cts.Token);
        cts.Token.ThrowIfCancellationRequested();
        
        return Ok();
    }
}