using Radzinsky.Framework.Routing;
using Telegram.Bot.Types;

namespace Radzinsky.Framework;

public interface IEndpoint
{
    public Task HandleAsync(Update update, Route route, CancellationToken cancellationToken);
}