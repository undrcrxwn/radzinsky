using HtmlAgilityPack;
using Radzinsky.Framework;
using Radzinsky.Framework.Routing;
using Radzinsky.Framework.Routing.RegEx;
using Radzinsky.Framework.Routing.StringDistance;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Radzinsky.Endpoints;

[RegExPatterns(
    @"(?i)^\/news(?:@radzinsky_bot)?$",
    @"(?i)^\/panorama(?:@radzinsky_bot)?$",
    @"(?i)^\/панорама(?:@radzinsky_bot)?$")]
[StringDistanceAliases("новости", "панорама", "вести", "что случилось", "сводка", "новостная сводка", "доложить обстановку")]
public class Panorama(ITelegramBotClient bot) : IEndpoint
{
    private const string NewsUrlTemplate = "https://panorama.pub/news/{0}";
    private const string NewsTitleSelector = "/html/body/div[3]/div/div[2]/a/div[3]";

    public async Task HandleAsync(Update update, Route route, CancellationToken cancellationToken)
    {
        var headlines = await GetHeadlinesAsync(DateTime.Today);
        var response = string.Join("\n", headlines.Select(headline => $"\u26a1 {headline}"));
        await bot.SendTextMessageAsync(
            chatId: update.Message!.Chat,
            text: response,
            cancellationToken: cancellationToken);
    }

    private static async Task<IEnumerable<string>> GetHeadlinesAsync(DateTime date)
    {
        var url = string.Format(NewsUrlTemplate, DateTime.Today.ToString("dd-MM-yyyy"));
        var document = await new HtmlWeb().LoadFromWebAsync(url);
        return document.DocumentNode
            .SelectNodes(NewsTitleSelector)
            .Select(node => node.InnerText.Trim());
    }
}