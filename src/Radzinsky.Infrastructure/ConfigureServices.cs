using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Radzinsky.Application.Abstractions;
using Radzinsky.Domain.Models;
using Radzinsky.Infrastructure.Services;

namespace Radzinsky.Infrastructure;

public static class ServiceCollectionExtensions
{
    private const string ResourcesPath = "resources.json";

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IStringDistanceMeasurer, DamerauLevenshteinSimilarityMeasurer>()
            .AddSingleton<IStringSimilarityMeasurer, StringSimilarityMeasurer>()
            .AddSingleton<ILinguisticParser, LinguisticParser>()
            .AddSingleton<IImperativeCallMapper, ImperativeCallMapper>()
            .AddSingleton<IImperativeArgumentParser, ImperativeArgumentParser>()
            .AddSingleton<UpdateReceiver>()
            .AddSingleton<IWebSearchService, GoogleSearchService>()
            .AddResources();
    }

    private static IServiceCollection AddResources(this IServiceCollection services)
    {
        var json = File.ReadAllText(ResourcesPath);
        var resources = JsonConvert.DeserializeObject<Resources>(json);
        return services.AddSingleton(resources);
    }
}