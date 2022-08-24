using MediatR;
using Telegram.Bot;

namespace Radzinsky.Bot.Requests;

public record MentionRequest(long ChatId) : IRequest;

internal class MentionRequestHandler : IRequestHandler<MentionRequest>
{
    private readonly ITelegramBotClient _bot;

    private const string MessageText = "К вашим услугам.";

    public MentionRequestHandler(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task<Unit> Handle(MentionRequest request, CancellationToken cancellationToken)
    {
        await _bot.SendTextMessageAsync(request.ChatId, MessageText, cancellationToken: cancellationToken);
        return Unit.Value;
    }
}