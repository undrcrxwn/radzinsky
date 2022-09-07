using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Host.Services;

public class ConfigureWebhook : IHostedService
{
    private readonly ILogger<ConfigureWebhook> _logger;
    private readonly IServiceProvider _services;
    private readonly BotConfiguration _botConfig;

    public ConfigureWebhook(ILogger<ConfigureWebhook> logger,
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
        _logger.LogInformation("Setting webhook: {WebhookAddress}", webhookAddress);
        await botClient.SetWebhookAsync(
            url: webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
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