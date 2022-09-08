using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Radzinsky.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration) =>
        services.AddNpgsql<ApplicationDbContext>(configuration.GetConnectionString("DefaultConnection"));
}