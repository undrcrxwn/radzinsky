using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Radzinsky.Framework.Configurations;
using Radzinsky.Framework.Routing.RegEx;
using Radzinsky.Framework.Routing.StringDistance;
using Telegram.Bot;

namespace Radzinsky.Framework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFramework(
        this IServiceCollection services,
        Action<FrameworkOptions.Builder> configureOptions)
    {
        var optionsBuilder = new FrameworkOptions.Builder();
        configureOptions(optionsBuilder);
        var options = optionsBuilder.Build();

        services.AddSingleton<RegExRouter>();
        services.AddSingleton(_ =>
        {
            var discovery = new RegExEndpointDiscovery();
            discovery.ScanAssemblies(options.AssembliesToScan);
            return discovery;
        });

        services.AddSingleton<DamerauLevenshteinStringDistanceCalculator>();
        services.AddSingleton<StringDistanceRouter>();
        services.AddSingleton(_ =>
        {
            var discovery = new StringDistanceEndpointDiscovery();
            discovery.ScanAssemblies(options.AssembliesToScan);
            return discovery;
        });

        var endpointTypes = options.AssembliesToScan
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsAssignableTo(typeof(IEndpoint)));

        foreach (var endpointType in endpointTypes)
            services.AddScoped(endpointType);

        services.AddSingleton<UpdateHandler>();
        return services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var configuration = provider.GetRequiredService<IOptions<TelegramConfiguration>>();
            return new TelegramBotClient(configuration.Value.Token);
        });
    }
}