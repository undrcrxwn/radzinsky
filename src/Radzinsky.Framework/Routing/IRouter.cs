using Telegram.Bot.Types;

namespace Radzinsky.Framework.Routing;

public interface IRouter
{
    public Route? TryMatchEndpoint(Update update);
}