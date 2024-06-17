using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Radzinsky.Persistence;

public class DatabaseMigrator(IServiceScopeFactory serviceScopeFactory, ILogger<DatabaseMigrator> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        logger.LogInformation("Initializing database");
        await context.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}