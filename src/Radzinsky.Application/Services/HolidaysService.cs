using HtmlAgilityPack;
using Radzinsky.Application.Abstractions;

namespace Radzinsky.Application.Services;

public class HolidaysService : IHolidaysService
{
    private const string HolidaysPageUrl = "https://kakoyprazdnik.com/";
    private const string HolidayTitleSelector = "//div[@id='mainzona']//div[@id='bloktxt']//h4";

    public async Task<IEnumerable<string>> GetHolidaysAsync()
    {
        var document = await new HtmlWeb().LoadFromWebAsync(HolidaysPageUrl);
        return document.DocumentNode
            .SelectNodes(HolidayTitleSelector)
            .Select(x => x.InnerText);
    }
}