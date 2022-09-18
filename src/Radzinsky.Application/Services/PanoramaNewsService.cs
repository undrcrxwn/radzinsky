using HtmlAgilityPack;
using Radzinsky.Application.Abstractions;

namespace Radzinsky.Application.Services;

public class PanoramaNewsService : INewsService
{
    private const string NewsUrlTemplate = "https://panorama.pub/news/{0}";
    private const string NewsTitleSelector = "/html/body/div[3]/div/div[2]/a/div[3]";
    
    public async Task<IEnumerable<string>> GetTitlesAsync(DateTime date)
    {
        var url = string.Format(NewsUrlTemplate, DateTime.Today.ToString("dd-MM-yyyy"));
        var document = await new HtmlWeb().LoadFromWebAsync(url);
        return document.DocumentNode
            .SelectNodes(NewsTitleSelector)
            .Select(x => x.InnerText.Trim());
    }
}