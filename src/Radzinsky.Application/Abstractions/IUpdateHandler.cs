using Telegram.Bot.Types;

namespace Radzinsky.Application.Abstractions;

public interface IUpdateHandler
{
    public Task HandleAsync(Update update, CancellationToken cancellationToken);
}