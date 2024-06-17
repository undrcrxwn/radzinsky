using Telegram.Bot.Types;

namespace Radzinsky.Framework.Routing.RegEx;

public class RegExRouter(RegExEndpointDiscovery discovery) : IRouter
{
    public Route? TryMatchEndpoint(Update update)
    {
        if (update.Message?.Text is not { } text)
            return null;

        var matchesByEndpointType = discovery.Endpoints.Select(endpoint => new
        {
            endpoint.EndpointType,
            FirstSuccessfulMatch = endpoint.Patterns
                .Select(pattern => pattern.Match(text))
                .FirstOrDefault(match => match.Success)
        });

        var match = matchesByEndpointType.FirstOrDefault(match => match.FirstSuccessfulMatch is not null);
        return match is not null
            ? new RegExRoute(match.EndpointType, match.FirstSuccessfulMatch!.Groups)
            : null;
    }
}