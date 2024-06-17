using System.Diagnostics;
using System.Globalization;
using System.Text;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Radzinsky.Framework;
using Radzinsky.Framework.Routing;
using Radzinsky.Framework.Routing.RegEx;
using Radzinsky.Framework.Routing.StringDistance;
using Radzinsky.Persistence;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Endpoints;

[RegExPatterns(@"(?i)^\/status(?:@radzinsky_bot)?$")]
[StringDistanceAliases("статус", "статистика", "аналитика", "анализ", "хелсчек")]
public class Status(
    ITelegramBotClient bot,
    DatabaseContext context,
    IHostEnvironment environment,
    RegExEndpointDiscovery regExDiscovery,
    StringDistanceEndpointDiscovery stringDistanceDiscovery) : IEndpoint
{
    public async Task HandleAsync(Update update, Route route, CancellationToken cancellationToken)
    {
        var response = new StringBuilder();


        response.AppendLine("*Среда*");

        response.AppendLine($"— Окружение: {environment.EnvironmentName}");
        response.AppendLine($"— ОС: {Environment.OSVersion}");
        response.AppendLine($"— Логических процессоров: {Environment.ProcessorCount}");
        response.AppendLine($"— Локаль: {CultureInfo.CurrentUICulture}");
        
        var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
        response.AppendLine($"— Аптайм процесса: {uptime.Humanize()}");


        response.AppendLine();
        response.AppendLine("*База данных*");
        response.AppendLine($"— Провайдер: {context.Database.ProviderName!.Split('.').Last()}");

        var chatCount = await context.Chats.CountAsync(cancellationToken);
        response.AppendLine($"— Чатов: {chatCount.ToString("N0")}");

        var userCount = await context.Users.CountAsync(cancellationToken);
        response.AppendLine($"— Юзеров: {userCount.ToString("N0")}");

        var chatMemberCount = await context.ChatMembers.CountAsync(cancellationToken);
        response.AppendLine($"— Чат-мемберов: {chatMemberCount.ToString("N0")}");

        var roleCount = await context.Roles.CountAsync(cancellationToken);
        response.AppendLine($"— Ролей: {roleCount.ToString("N0")}");


        response.AppendLine();
        response.AppendLine("*Эндпоинты*");

        var regExPatternCount = regExDiscovery.Endpoints.Sum(endpoint => endpoint.Patterns.Count);
        response.AppendLine($"— RegEx паттернов: {regExPatternCount}");

        var stringDistanceAliasCount = stringDistanceDiscovery.Endpoints.Sum(endpoint => endpoint.Aliases.Count);
        response.AppendLine($"— Строковых псевдонимов: {stringDistanceAliasCount}");


        response.AppendLine();
        response.AppendLine("*Перфоманс*");

        var gcMemory = GC.GetTotalMemory(forceFullCollection: false).Bytes();
        response.AppendLine($"— GC без фрагментации: {gcMemory}");

        var gcMemoryInfo = GC.GetGCMemoryInfo();
        response.AppendLine($"— Зарезервировано GC: {gcMemoryInfo.TotalCommittedBytes.Bytes()}");

        var privateMemory = Process.GetCurrentProcess().PrivateMemorySize64.Bytes();
        response.AppendLine($"— Private Bytes: {privateMemory}");

        var workingSetMemory = Process.GetCurrentProcess().WorkingSet64.Bytes();
        response.AppendLine($"— Working Set: {workingSetMemory}");

        response.AppendLine($"— Объектов, ожидающих финализацию: {gcMemoryInfo.FinalizationPendingCount}");
        response.AppendLine($"— Времени тратится на GC: {gcMemoryInfo.PauseTimePercentage}%");
        response.AppendLine($"— Потоков в тред-пуле: {ThreadPool.ThreadCount}");


        await bot.SendTextMessageAsync(
            chatId: update.Message!.Chat,
            text: response.ToString(),
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
}