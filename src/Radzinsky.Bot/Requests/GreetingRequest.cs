using MediatR;
using Serilog;
using Radzinsky.Bot.Common;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Bot.Requests;

public record GreetingRequest(
        long ChatId, bool HasMention, ChatType ChatType,
        long UserId, string FirstName)
    : MessageRequestBase(ChatId, HasMention, ChatType);

public class GreetingRequestHandler
{
    private readonly ITelegramBotClient _bot;

    public GreetingRequestHandler(ITelegramBotClient bot)
    {
        _bot = bot;
    }
    
    public async Task<Unit> Handle(GreetingRequest request, CancellationToken cancellationToken)
    {
        await _bot.SendTextMessageAsync(request.ChatId, "Привет!", cancellationToken: cancellationToken);
        return Unit.Value;
    }
}
