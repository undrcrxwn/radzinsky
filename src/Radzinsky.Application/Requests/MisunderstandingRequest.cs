using MediatR;
using Telegram.Bot;

namespace Radzinsky.Application.Requests;

public record MisunderstandingRequest(long ChatId) : IRequest;

internal class MisunderstandingRequestHandler : IRequestHandler<MisunderstandingRequest>
{
    private readonly ITelegramBotClient _bot;

    private const string MessageText = "Ничего не пойму. Переформулируй.";

    public MisunderstandingRequestHandler(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task<Unit> Handle(MisunderstandingRequest request, CancellationToken cancellationToken)
    {
        await _bot.SendTextMessageAsync(request.ChatId, MessageText, cancellationToken: cancellationToken);
        return Unit.Value;
    }
}