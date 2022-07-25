using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Radzinsky.Application;
using Radzinsky.Infrastructure;
using Radzinsky.Infrastructure.Services;
using Serilog;

namespace Radzinsky.Bot;

public class UpdatePipelineBuilder
{
}

public static class Program
{
    private const string AppSettingsPath = "appsettings.json";

    public static void Main(string[] args)
    {
        ConfigureLogging();

        var host = CreateHostBuilder(args).Build();
        host.Start();

        var pipeline = host.Services.GetRequiredService<UpdateReceiver>();
        var cts = new CancellationTokenSource();
        pipeline.StartReceiving(cts.Token);

        host.WaitForShutdown();
    }

    private static void ConfigureLogging()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            Log.Fatal("Unhandled exception: {0}", e.ExceptionObject);

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(AppSettingsPath)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .CreateLogger();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices(services => services
                .AddApplicationServices()
                .AddInfrastructureServices()
                .AddTelegramBot());
    }
}