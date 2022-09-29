using Telegram.Bot;

namespace Radzinsky.Host.Services;

public class WebhookConfigurator : IHostedService
{
    private readonly ILogger<WebhookConfigurator> _logger;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;

    public WebhookConfigurator(
        ILogger<WebhookConfigurator> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _services = serviceProvider;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        
        var hostAddress = _configuration["HOST_ADDRESS"];
        var token = _configuration["Telegram:BotApiToken"];
        var webhookAddress = @$"{hostAddress}/bot/{token}";
        
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