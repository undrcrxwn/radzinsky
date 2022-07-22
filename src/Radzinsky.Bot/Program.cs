using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Radzinsky.Application;
using Serilog;

namespace Radzinsky.Bot;

public static class Program
{
    private const string AppSettingsPath = "appsettings.json";

    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            Log.Fatal("Unhandled exception: {0}", e.ExceptionObject);

        var host = CreateHostBuilder(args).Build();
        host.Start();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
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

        var hostBuilder = Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices(services => services
                .AddTelegramBot()
                .AddApplicationServices());

        return hostBuilder;
    }
}