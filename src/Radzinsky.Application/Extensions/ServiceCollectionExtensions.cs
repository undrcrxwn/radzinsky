using System.Globalization;
using System.Reflection;
using CsvHelper;
using Hangfire;
using Hangfire.PostgreSql;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;
using Radzinsky.Application.Services;
using Radzinsky.Domain.Models;
using Serilog;

namespace Radzinsky.Application.Extensions;

public static class ServiceCollectionExtensions
{
    private const string CommandResourcesPath = "Resources/commands.json";
    private const string SelfResourcesPath = "Resources/self.json";
    private const string RussianBigramFrequenciesPath = "Resources/russian_bigram_frequencies.csv";
    private const string EnglishBigramFrequenciesPath = "Resources/english_bigram_frequencies.csv";

    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSelfResources()
            .AddMemoryCache()
            .AddHangfire(configuration)
            .AddCommandResources()
            .AddCommands()
            .AddLinguisticParsing()
            .AddBigramFrequencies()
            .AddMapsterConfiguration()
            .AddScoped<IUpdateHandler, UpdateHandler>()
            .AddSingleton<IWebSearchService, GoogleSearchService>()
            .AddTransient<IRuntimeInfoService, RuntimeInfoService>()
            .AddSingleton<IInteractionService, InteractionService>()
            .AddSingleton<ICommandsService, CommandsService>()
            .AddSingleton<IHolidaysService, HolidaysService>()
            .AddSingleton<IKeyboardLayoutTranslator, KeyboardLayoutTranslator>()
            .AddScoped<CommandContext>();

    private static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<Telegram.Bot.Types.Message, Message>.NewConfig()
            .Map(
                destination => destination.Sender,
                source => source.From.Adapt<User>())
            .Map(
                destination => destination.ReplyTarget,
                source => source.ReplyToMessage.Adapt<Message>());

        return services;
    }
    
    private static IServiceCollection AddCommands(this IServiceCollection services)
    {
        var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(ICommand)));

        foreach (var commandType in commandTypes)
        {
            Log.Information("Registering command of type {0}", commandType.FullName);
            services.AddScoped(commandType);
        }

        return services;
    }

    private static IServiceCollection AddCommandResources(this IServiceCollection services) =>
        services.AddSingleton(DeserializeFromRelativeLocation<IEnumerable<CommandResources>>(CommandResourcesPath));

    private static IServiceCollection AddSelfResources(this IServiceCollection services) =>
        services.AddSingleton(DeserializeFromRelativeLocation<SelfResources>(SelfResourcesPath));

    private static T DeserializeFromRelativeLocation<T>(string relativePath)
    {
        var absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        var json = File.ReadAllText(absolutePath);
        return JsonConvert.DeserializeObject<T>(json);
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
            .ToDictionary(x => x.Bigram, x => (double)x.Frequency);
    }

    private record BigramFrequency(string Bigram, double Frequency);
}