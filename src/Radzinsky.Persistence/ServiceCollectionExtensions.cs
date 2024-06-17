using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Radzinsky.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryPersistence(this IServiceCollection services) =>
        services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(nameof(DatabaseContext)));

    public static IServiceCollection AddPostgresPersistence(this IServiceCollection services, IConfiguration configuration) => services
        .AddDbContext<DatabaseContext>(options => options.UseNpgsql(configuration.GetConnectionString(nameof(DatabaseContext))))
        .AddHostedService<DatabaseMigrator>();
}