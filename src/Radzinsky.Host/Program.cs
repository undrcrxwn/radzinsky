using System.Globalization;
using Radzinsky.Endpoints;
using Radzinsky.Framework;
using Radzinsky.Framework.Configurations;
using Radzinsky.Host;
using Radzinsky.Host.Transport;
using Radzinsky.Persistence;

CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddBoundConfigurations(builder.Configuration);
builder.Services.AddFramework(options => options.ScanAssembly(typeof(Start).Assembly));

// In Development environment, EF Core's InMemory provider is preferred over Npgsql
if (builder.Environment.IsDevelopment())
    builder.Services.AddInMemoryPersistence();
else
    builder.Services.AddPostgresPersistence(builder.Configuration);

var telegramConfiguration = new TelegramConfiguration();
builder.Configuration.GetSection("Telegram").Bind(telegramConfiguration);
var preferWebhookTransport = telegramConfiguration.WebhookHost is not null;

// Use webhook instead of long polling
if (preferWebhookTransport)
{
    builder.Services.AddControllers().AddNewtonsoftJson();
    builder.Services.AddHostedService<WebhookInitializer>();
}
else
{
    builder.Services.AddHostedService<LongPollingInitializer>();
}

var app = builder.Build();

// Configure middleware pipeline
app.UseHealthChecks("/healthz");
if (preferWebhookTransport)
    app.MapControllers();

await app.RunAsync();