using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Telegram.Bot;

namespace Radzinsky.Bot;

public static class ServiceCollectionExtensions
{
    private const string BotTokenVariableKey = "BOT_TOKEN";

    public static IServiceCollection AddTelegramBot(this IServiceCollection services)
    {
        var token =
            Environment.GetEnvironmentVariable(BotTokenVariableKey)
            ?? throw new Exception($"{BotTokenVariableKey} environment variable is not set.");

        Log.Information("Logging into bot...");
        var bot = new TelegramBotClient(token);
        var me = bot.GetMeAsync().GetAwaiter().GetResult();
        Log.Information("Telegram bot client is all set. Working on {0} ({1})",
            me.FirstName, me.Id);

        return services.AddSingleton<ITelegramBotClient>(bot);
    }
}