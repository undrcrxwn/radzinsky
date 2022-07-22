using MediatR;
using Telegram.Bot;

namespace Radzinsky.Application.Requests;

public record StartRequest(long ChatId) : IRequest;

internal class StartRequestHandler : IRequestHandler<StartRequest>
{
    private readonly ITelegramBotClient _bot;

    private const string MessageText =
        "Меня звать Радзински. Рафик Радзински. Но вообще можно просто Раф, — не суть.";

    public StartRequestHandler(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task<Unit> Handle(StartRequest request, CancellationToken cancellationToken)
    {
        await _bot.SendTextMessageAsync(request.ChatId, MessageText, cancellationToken: cancellationToken);
        return Unit.Value;
    }
}