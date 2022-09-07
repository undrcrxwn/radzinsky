using Radzinsky.Application.Extensions;
using Radzinsky.Host;
using Radzinsky.Host.Services;
using Serilog;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();

builder.Services.AddHostedService<ConfigureWebhook>();

builder.Services.AddHttpClient("tgwebhook")
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botConfig.BotToken, httpClient));

builder.Services
    .AddApplicationServices()
    .AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

app.UseRouting();
app.UseCors();

app.UseEndpoints(endpoints =>
{
    var token = botConfig.BotToken;
    endpoints.MapControllerRoute(
        name: "tgwebhook", pattern: $"bot/{token}",
        new { controller = "Webhook", action = "Post" });
    endpoints.MapControllers();
});

app.Run();