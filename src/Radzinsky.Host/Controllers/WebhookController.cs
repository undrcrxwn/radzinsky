using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Radzinsky.Framework;
using Radzinsky.Framework.Configurations;
using Radzinsky.Framework.Exceptions;
using Telegram.Bot.Types;

namespace Radzinsky.Host.Controllers;

[ApiController]
public class WebhookController(
    UpdateHandler handler,
    IOptions<TelegramConfiguration> configuration,
    ILogger<WebhookController> logger)
    : ControllerBase
{
    [HttpPost("~/bot/{token}")]
    public async Task<IActionResult> ReceiveUpdateAsync([FromRoute] string token, Update update)
    {
        if (token != configuration.Value.Token)
            return Forbid("The specified Telegram Bot API token is invalid. Get the correct one from @BotFather.");

        try
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await handler.HandleAsync(update, cts.Token);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Exception was thrown when handling update");
        }

        return Ok();
    }
}