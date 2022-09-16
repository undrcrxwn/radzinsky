﻿using System.Reflection;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;
using Radzinsky.Application.Services;
using Serilog;

namespace Radzinsky.Application.Extensions;

public static class ServiceCollectionExtensions
{
    private const string CommandResourcesPath = "Resources/commands.json";
    private const string SelfResourcesPath = "Resources/self.json";

    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration) => services
        .AddSelfResources()
        .AddMemoryCache()
        .AddHangfire(configuration)
        .AddCommandResources()
        .AddCommands()
        .AddLinguisticParsing()
        .AddScoped<IUpdateHandler, UpdateHandler>()
        .AddSingleton<IWebSearchService, GoogleSearchService>()
        .AddTransient<IRuntimeInfoService, RuntimeInfoService>()
        .AddSingleton<IInteractionService, InteractionService>()
        .AddSingleton<ICommandsService, CommandsService>()
        .AddSingleton<IHolidaysService, HolidaysService>()
        .AddScoped<CommandContext>();

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

    private static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration) => services
        .AddHangfire(options =>
            options.UsePostgreSqlStorage(configuration.GetConnectionString("HangfireConnection")))
        .AddHangfireServer();
}