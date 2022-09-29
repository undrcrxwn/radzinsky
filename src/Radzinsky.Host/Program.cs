using Hangfire;
using Hangfire.AspNetCore;
using Radzinsky.Application.Extensions;
using Radzinsky.Host.Services;
using Radzinsky.Persistence.Extensions;
using Serilog;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.AppSettings()
    .WriteTo.Console()
    .WriteTo.File("logs/session.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddHostedService<WebhookConfigurator>();

var token = builder.Configuration["Telegram:BotApiToken"];
builder.Services.AddHttpClient("tgwebhook")
    .AddTypedClient<ITelegramBotClient>(client => new TelegramBotClient(token, client));

builder.Services
    .AddApplication(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

app.MapControllers();
app.UseRouting();
app.UseCors();

var factory = app.Services.GetRequiredService<IServiceScopeFactory>();
GlobalConfiguration.Configuration.UseActivator(
    new AspNetCoreJobActivator(factory));

app.UseHangfireDashboard();

app.Run();