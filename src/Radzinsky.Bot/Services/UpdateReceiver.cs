using MediatR;
using Radzinsky.Bot.Requests;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Bot.Services;

public class UpdateReceiver
{
    private readonly ITelegramBotClient _bot;
    private readonly IMediator _mediator;

    public UpdateReceiver(ITelegramBotClient bot, IMediator mediator)
    {
        _bot = bot;
        _mediator = mediator;
    }

    public void StartReceiving(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = true
        };

        _bot.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions,
            cancellationToken
        );

        Log.Information("Update pipeline is running");
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Message?.Text is null)
            return;

#if DEBUG
        Log.Debug("Received message: {@0}", update.Message);
#else
        Log.Information("Received message ({0}) from chat {1}: {2}",
            update.Message.MessageId, update.Message.Chat.Id, update.Message.Text);
#endif

        var isDirectMessage = update.Message.Chat.Type == ChatType.Private;
        var request = new LinguisticRequest(update.Message.Text, isDirectMessage, update.Message);

        try
        {
            await _mediator.Send(request);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unhandled exception raised while processing message: {@0}", update.Message);
        }
    }

    private Task HandlePollingErrorAsync(
        ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Log.Error("Polling error: {0}", exception);
        return Task.CompletedTask;
    }
}