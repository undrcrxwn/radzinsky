﻿using Telegram.Bot;

namespace Radzinsky.Host.Services;

public class WebhookConfigurator : IHostedService
{
    private readonly ILogger<WebhookConfigurator> _logger;
    private readonly IServiceProvider _services;
    private readonly BotConfiguration _botConfig;

    public WebhookConfigurator(ILogger<WebhookConfigurator> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _services = serviceProvider;
        _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var webhookAddress = @$"{_botConfig.HostAddress}/bot/{_botConfig.BotToken}";
        _logger.LogInformation("Setting webhook: {0}", webhookAddress);
        await botClient.SetWebhookAsync(
            url: webhookAddress,
            dropPendingUpdates: true,
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        // Remove webhook upon app shutdown
        _logger.LogInformation("Removing webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}