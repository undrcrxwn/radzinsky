using Microsoft.AspNetCore.Mvc;
using Radzinsky.Application.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Radzinsky.Host.Controllers;

[ApiController, Route("bot")]
public class WebhookController : ControllerBase
{
    [HttpPost("{token}")]
    public async Task<IActionResult> ReceiveUpdateAsync(
        [FromServices] IUpdateHandler updateHandler,
        [FromBody] Update update)
    {
        var cts = new CancellationTokenSource();
        await updateHandler.HandleAsync(update, cts.Token);
        cts.Token.ThrowIfCancellationRequested();
        
        return Ok();
    }
}
