using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Attributes;
using Radzinsky.Application.Behaviors;
using Serilog;

namespace Radzinsky.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddMediatR(Assembly.GetExecutingAssembly())
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddImperativeCallMappings()
            .AddImperativeArgumentParsingStrategies();
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
}