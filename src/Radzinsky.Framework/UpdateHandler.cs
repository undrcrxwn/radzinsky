using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using Radzinsky.Framework.Routing.RegEx;
using Radzinsky.Framework.Routing.StringDistance;

namespace Radzinsky.Framework;

public class UpdateHandler(
    RegExRouter regExRouter,
    StringDistanceRouter stringDistanceRouter,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<UpdateHandler> logger)
{
    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "Received update of type {UpdateType} from chat {ChatId} with message: {MessageText}",
            update.Type, update.Message?.Chat.Id, update.Message?.Text);

        var route =
            regExRouter.TryMatchEndpoint(update) ??
            stringDistanceRouter.TryMatchEndpoint(update);
        
        if (route is null)
            return;

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var endpoint = (IEndpoint)scope.ServiceProvider.GetRequiredService(route.EndpointType);
        await endpoint.HandleAsync(update, route, cancellationToken);
    }
}