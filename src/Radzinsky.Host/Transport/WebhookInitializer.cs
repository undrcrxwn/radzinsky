using Microsoft.Extensions.Options;
using Radzinsky.Framework.Configurations;
using Telegram.Bot;

namespace Radzinsky.Host.Transport;

public class WebhookInitializer(
    ITelegramBotClient bot,
    IOptions<TelegramConfiguration> configuration,
    ILogger<WebhookInitializer> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var webhookUrl = $"{configuration.Value.WebhookHost}/bot/{configuration.Value.Token}";

        logger.LogInformation("Setting webhook at {Url}", webhookUrl);
        await bot.SetWebhookAsync(
            url: webhookUrl,
            dropPendingUpdates: true,
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing webhook");
        await bot.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}