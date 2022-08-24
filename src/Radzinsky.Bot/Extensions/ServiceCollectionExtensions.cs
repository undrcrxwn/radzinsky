using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Radzinsky.Bot.Abstractions;
using Radzinsky.Bot.Attributes;
using Radzinsky.Bot.Behaviors;
using Radzinsky.Bot.Models;
using Radzinsky.Bot.Services;
using Serilog;
using Telegram.Bot;

namespace Radzinsky.Bot.Extensions;

public static class ServiceCollectionExtensions
{
    private const string BotTokenVariableKey = "BOT_TOKEN";
    private const string ResourcesPath = "resources.json";

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddTelegramBot()
            .AddMediator()
            .AddResources()
            .AddImperatives()
            .AddLinguistics()
            .AddThirdPartyDependencies();
    }

    private static IServiceCollection AddTelegramBot(this IServiceCollection services)
    {
        var token =
            Environment.GetEnvironmentVariable(BotTokenVariableKey)
            ?? throw new Exception($"{BotTokenVariableKey} environment variable is not set.");

        Log.Information("Logging into bot...");
        var bot = new TelegramBotClient(token);
        var me = bot.GetMeAsync().GetAwaiter().GetResult();
        Log.Information("Telegram bot client is all set. Working on {0} ({1})",
            me.FirstName, me.Id);

        return services
            .AddSingleton<ITelegramBotClient>(bot)
            .AddSingleton<UpdateReceiver>();
    }

    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        return services
            .AddMediatR(Assembly.GetExecutingAssembly())
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    }

    private static IServiceCollection AddImperatives(this IServiceCollection services)
    {
        return services
            .AddImperativeCallMappings()
            .AddImperativeArgumentParsingStrategies()
            .AddSingleton<IImperativeCallMapper, ImperativeCallMapper>()
            .AddSingleton<IImperativeArgumentParser, ImperativeArgumentParser>();
    }

    private static IServiceCollection AddImperativeCallMappings(this IServiceCollection services)
    {
        return services.AddUserTypes<IImperativeCallMapping, ImperativeCallMappingAttribute>();
    }

    private static IServiceCollection AddImperativeArgumentParsingStrategies(this IServiceCollection services)
    {
        return services.AddUserTypes<IImperativeArgumentParsingStrategy, ImperativeArgumentParsingStrategyAttribute>();
    }

    private static IServiceCollection AddUserTypes<T, TAttribute>(this IServiceCollection services)
        where TAttribute : Attribute
    {
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => typeof(T).IsAssignableFrom(x) &&
                        x.GetCustomAttribute<TAttribute>() is not null);

        foreach (var type in types)
        {
            services.AddSingleton(typeof(T), type);
            Log.Information("{0} has been registered as user implementation of {1} with required attribute {2}",
                type.Name, typeof(T).Name, typeof(TAttribute).Name);
        }

        return services;
    }

    private static IServiceCollection AddResources(this IServiceCollection services)
    {
        var json = File.ReadAllText(ResourcesPath);
        var resources = JsonConvert.DeserializeObject<Resources>(json);
        return services.AddSingleton(resources);
    }

    private static IServiceCollection AddLinguistics(this IServiceCollection services)
    {
        return services
            .AddSingleton<IStringDistanceMeasurer, DamerauLevenshteinSimilarityMeasurer>()
            .AddSingleton<IStringSimilarityMeasurer, StringSimilarityMeasurer>()
            .AddSingleton<ILinguisticParser, LinguisticParser>();
    }

    private static IServiceCollection AddThirdPartyDependencies(this IServiceCollection services)
    {
        return services.AddSingleton<IWebSearchService, GoogleSearchService>();
    }
}