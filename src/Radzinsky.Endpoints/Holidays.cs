using HtmlAgilityPack;
using Radzinsky.Framework;
using Radzinsky.Framework.Routing;
using Radzinsky.Framework.Routing.RegEx;
using Radzinsky.Framework.Routing.StringDistance;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Radzinsky.Endpoints;

[RegExPatterns(@"(?i)^\/holidays(?:@radzinsky_bot)?$")]
[StringDistanceAliases(
    "праздники", "праздники сегодня", "праздники в этот день", "что сегодня случилось", "что сегодня произошло", "сегодняшние события",
    "сегодняшние праздники", "что празднуем", "памятные даты", "календарь", "что по праздникам", "что сегодня за праздники",
    "что празднуем сегодня", "что сегодня празднуем", "какие сегодня праздники", "что празднуем")]
public class Holidays(ITelegramBotClient bot) : IEndpoint
{
    private const string HolidaysPageUrl = "https://kakoyprazdnik.com/";
    private const string HolidayTitleSelector = "//div[@id='mainzona']//div[@id='bloktxt']//h4";

    public async Task HandleAsync(Update update, Route route, CancellationToken cancellationToken)
    {
        var document = await new HtmlWeb().LoadFromWebAsync(HolidaysPageUrl, cancellationToken);
        var holidayNames = document.DocumentNode.SelectNodes(HolidayTitleSelector).Select(node => node.InnerText);
        var decoratedHolidayNames = holidayNames.Select(holiday => $"\ud83c\udf7e {holiday}");

        await bot.SendTextMessageAsync(
            chatId: update.Message!.Chat,
            text: string.Join('\n', decoratedHolidayNames),
            disableWebPagePreview: true,
            cancellationToken: cancellationToken);
    }
}