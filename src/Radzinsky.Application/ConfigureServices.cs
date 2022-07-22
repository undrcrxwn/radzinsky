using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Behaviors;

namespace Radzinsky.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddMediatR(Assembly.GetExecutingAssembly())
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    }
}