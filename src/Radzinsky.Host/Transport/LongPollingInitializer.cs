using Radzinsky.Framework;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Radzinsky.Host.Transport;

public class LongPollingInitializer(
    ITelegramBotClient bot,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<LongPollingInitializer> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting long polling");
        var receiverOptions = new ReceiverOptions { ThrowPendingUpdates = true };
        await bot.ReceiveAsync(OnUpdateReceived, OnPollingErrorThrown, receiverOptions, cancellationToken);
    }

    private async Task OnUpdateReceived(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var updateHandler = scope.ServiceProvider.GetRequiredService<UpdateHandler>();

        try
        {
            await updateHandler.HandleAsync(update, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Exception was thrown when handling update");
        }
    }

    private Task OnPollingErrorThrown(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Long polling error was thrown");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}