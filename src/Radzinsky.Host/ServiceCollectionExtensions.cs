using Radzinsky.Framework.Configurations;

namespace Radzinsky.Host;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBoundConfigurations(this IServiceCollection services, IConfiguration configuration) => services
        .Configure<TelegramConfiguration>(configuration.GetSection("Telegram"))
        .Configure<Radzinsky.Endpoints.Google.Configuration>(configuration.GetSection("Google"));
}