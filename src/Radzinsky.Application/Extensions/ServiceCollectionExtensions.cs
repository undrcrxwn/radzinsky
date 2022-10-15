using System.Globalization;
using System.Reflection;
using CsvHelper;
using Hangfire;
using Hangfire.PostgreSql;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Behaviors;
using Radzinsky.Application.Models;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Models.DTOs;
using Radzinsky.Application.Models.Resources;
using Radzinsky.Application.Services;
using Serilog;

namespace Radzinsky.Application.Extensions;

public static class ServiceCollectionExtensions
{
    private const string CommonResourcesPath = "Resources/common.json";
    private const string CommandResourcesPathTemplate = "Resources/Commands/{0}.json";
    private const string BehaviorResourcesPathTemplate = "Resources/Behaviors/{0}.json";
    private const string RussianBigramFrequenciesPath = "Resources/Linguistic/russian_bigram_frequencies.csv";
    private const string EnglishBigramFrequenciesPath = "Resources/Linguistic/english_bigram_frequencies.csv";

    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddMemoryCache()
            .AddMapsterConfiguration(configuration)
            .AddHangfire(configuration)
            .AddBehaviorsAndResources()
            .AddCommandsAndResources()
            .AddCallbackQueryHandlers()
            .AddCommonResources()
            .AddLinguisticParsing()
            .AddBigramFrequencies()
            .AddScoped<IUpdateHandler, UpdateHandler>()
            .AddScoped<IResourcesService, ResourcesService>()
            .AddSingleton<IWebSearchService, GoogleSearchService>()
            .AddTransient<IRuntimeInfoService, RuntimeInfoService>()
            .AddSingleton<ICheckpointMemoryService, CheckpointMemoryService>()
            .AddSingleton<ICommandsService, CommandsService>()
            .AddSingleton<IHolidaysService, HolidaysService>()
            .AddSingleton<INewsService, PanoramaNewsService>()
            .AddSingleton<IKeyboardLayoutTranslator, KeyboardLayoutTranslator>()
            .AddSingleton<ICalculator, Calculator>()
            .AddSingleton<IHashingService, Md5HashingService>()
            .AddSingleton<IReplyMemoryService, ReplyMemoryService>()
            .AddScoped<IStateService, StateService>()
            .AddScoped<BehaviorContext>()
            .AddScoped<CommandContext>();

    private static IServiceCollection AddMapsterConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var keyLength = configuration.GetValue<int>("Callbacks:CallbackHandlerKeyLength");
        
        TypeAdapterConfig<Telegram.Bot.Types.Update, UpdateDto>.NewConfig()
            .Map(
                destination => destination.CallbackQuery,
                source => source.CallbackQuery == null
                    ? null
                    : source.CallbackQuery.Adapt<CallbackQueryDto>());

        TypeAdapterConfig<Telegram.Bot.Types.CallbackQuery, CallbackQueryDto>.NewConfig()
            .Map(
                destination => destination.CallbackHandlerTypeNameHash,
                source => source.Data == null
                    ? null
                    : source.Data.Substring(0, keyLength))
            .Map(
                destination => destination.CallbackHandlerTypeNameHash,
                source => source.Data == null
                    ? null
                    : source.Data.Substring(keyLength));
        
        TypeAdapterConfig<Telegram.Bot.Types.Message, MessageDto>.NewConfig()
            .Map(
                destination => destination.Sender,
                source => source.From == null
                    ? null
                    : source.From.Adapt<UserDto>())
            .Map(
                destination => destination.ReplyTarget,
                source => source.ReplyToMessage == null
                    ? null
                    : source.ReplyToMessage.Adapt<MessageDto>());

        return services;
    }

    private static IServiceCollection AddBehaviorsAndResources(this IServiceCollection services)
    {
        // Register behaviors manually to preserve their order
        var implementations = new[]
        {
            typeof(ErrorBehavior),
            typeof(SlashCommandBehavior),
            typeof(LinguisticCommandBehavior),
            typeof(MentionBehavior),
            typeof(WrongKeyboardLayoutBehavior),
            typeof(MisunderstandingBehavior)
        };

        foreach (var implementation in implementations)
        {
            services.AddScoped(typeof(IBehavior), implementation);
            services.AddScoped(implementation, implementation);
        }

        var behaviorTypes = GetImplementationsOf<IBehavior>();
        foreach (var behaviorType in behaviorTypes)
        {
            if (!behaviorType.IsAbstract && services.All(x => x.ImplementationType != behaviorType))
                Log.Warning("Behavior of type {0} is not registered", behaviorType.FullName);
        }

        return services.AddBehaviorResources(implementations.Select(x => x.FullName!));
    }

    private static IServiceCollection AddCommandsAndResources(this IServiceCollection services)
    {
        var commandTypes = GetImplementationsOf<ICommand>().ToArray();
        foreach (var commandType in commandTypes)
        {
            Log.Information("Registering command of type {0}", commandType.FullName);
            services.AddScoped(commandType);
        }

        return services.AddCommandResources(commandTypes.Select(x => x.FullName!));
    }

    private static IServiceCollection AddCallbackQueryHandlers(this IServiceCollection services)
    {
        var handlerTypes = GetImplementationsOf<ICallbackQueryHandler>().ToArray();
        foreach (var handlerType in handlerTypes)
        {
            Log.Information("Registering callback query handler of type {0}", handlerType.FullName);
            services.AddScoped(typeof(ICallbackQueryHandler), handlerType);
        }

        return services;
    }

    private static IServiceCollection AddCommandResources(
        this IServiceCollection services, IEnumerable<string> commandTypeNames)
    {
        var resourceMap = commandTypeNames.ToDictionary(
            commandTypeName => commandTypeName, commandTypeName =>
            {
                var path = string.Format(
                    CommandResourcesPathTemplate, commandTypeName.Split('.').Last());

                var data = ParseJObjectFromRelativeLocation(path);
                return data is not null
                    ? new CommandResources(ParseJObjectFromRelativeLocation(path))
                    : null;
            })
            .Where(x => x.Value is not null)
            .ToDictionary(x => x.Key, x => x.Value);;

        return services.AddSingleton<IDictionary<string, CommandResources?>>(resourceMap);
    }

    private static IServiceCollection AddBehaviorResources(
        this IServiceCollection services, IEnumerable<string> behaviorTypeNames)
    {
        var resourceMap = behaviorTypeNames.ToDictionary(
            behaviorTypeName => behaviorTypeName, behaviorTypeName =>
            {
                var path = string.Format(
                    BehaviorResourcesPathTemplate, behaviorTypeName.Split('.').Last());

                var data = ParseJObjectFromRelativeLocation(path);
                return data is not null
                    ? new BehaviorResources(data)
                    : null;
            })
            .Where(x => x.Value is not null)
            .ToDictionary(x => x.Key, x => x.Value);

        return services.AddSingleton<IDictionary<string, BehaviorResources?>>(resourceMap);
    }

    private static IServiceCollection AddCommonResources(this IServiceCollection services) =>
        services.AddSingleton(new CommonResources(ParseJObjectFromRelativeLocation(CommonResourcesPath)));

    private static JObject? ParseJObjectFromRelativeLocation(string relativePath)
    {
        var absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

        if (!File.Exists(absolutePath))
            return null;

        var json = File.ReadAllText(absolutePath);
        return JObject.Parse(json);
    }

    private static IServiceCollection AddLinguisticParsing(this IServiceCollection services) => services
        .AddSingleton<IStringDistanceMeasurer, DamerauLevenshteinSimilarityMeasurer>()
        .AddSingleton<IStringSimilarityMeasurer, StringSimilarityMeasurer>()
        .AddSingleton<ILinguisticParser, LinguisticParser>();

    private static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddHangfire(options =>
                options.UsePostgreSqlStorage(configuration.GetConnectionString("HangfireConnection")))
            .AddHangfireServer();

    private static IServiceCollection AddBigramFrequencies(this IServiceCollection services) =>
        services.AddSingleton(new BigramFrequencies
        {
            RussianBigramFrequencies = ReadFrequencies(RussianBigramFrequenciesPath),
            EnglishBigramFrequencies = ReadFrequencies(EnglishBigramFrequenciesPath)
        });

    private static IDictionary<string, double> ReadFrequencies(string relativePath)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        using var streamReader = File.OpenText(path);
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

        return csvReader.GetRecords<BigramFrequency>()
            .ToDictionary(x => x.Bigram, x => x.Frequency);
    }

    private static IEnumerable<Type> GetImplementationsOf<T>() where T : class =>
        Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(T)));

    private record BigramFrequency(string Bigram, double Frequency);
}