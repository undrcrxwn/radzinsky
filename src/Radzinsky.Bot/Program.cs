using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;

namespace Radzinsky.Bot;

internal interface IHelloWorldService
{
    void SayHelloWorld();
}

internal class HelloWorldService : IHelloWorldService
{
    public void SayHelloWorld()
    {
        Log.Information("Hello world!");
    }
}

public record StartRequest(string Username) : IRequest;

public class StartHandler : IRequestHandler<StartRequest>
{
    public Task<Unit> Handle(StartRequest request, CancellationToken cancellationToken)
    {
        Log.Information("Start from {0} handled", $"@{request.Username}");
        return Task.FromResult(Unit.Value);
    }
}

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        TResponse? response = default;
        try
        {
            Log.Debug("Handling {@0}", request);
            response = await next();
            return response;
        }
        finally
        {
            Log.Debug("{0} handled with return of {@1}", typeof(TRequest).Name, response);
        }
    }
}

public static class Program
{
    private const string BotTokenVariableKey = "BOT_TOKEN";
    private const string AppSettingsPath = "appsettings.json";
    
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            Log.Fatal("{0}", args.ExceptionObject);
        
        var builder = CreateHostBuilder(args);

        var token = Environment.GetEnvironmentVariable(BotTokenVariableKey)
            ?? throw new Exception($"{BotTokenVariableKey} environment variable is not set.");
        Log.Information("Logging into bot...");
        var bot = new TelegramBotClient(token);
        builder.ConfigureServices(services => services
            .AddSingleton<ITelegramBotClient>(bot));
        
        var host = builder.Build();
        host.Start();

        var mediator = host.Services.GetRequiredService<IMediator>();
        
        mediator.Send(new StartRequest("undrcrxwn")).Wait();
        
        var me = bot.GetMeAsync().GetAwaiter().GetResult();
        Log.Information("Telegram bot client is all set. Working on {0} ({1})",
            me.FirstName, me.Id);
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
                .AddMediatR(typeof(Program))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddSingleton<IHelloWorldService, HelloWorldService>());

        return hostBuilder;
    }
}
