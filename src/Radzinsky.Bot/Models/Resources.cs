using Radzinsky.Bot.Enumerations;

namespace Radzinsky.Bot.Models;

public class Resources
{
    public IDictionary<PlainTextType, IEnumerable<string>> PlainTextCases { get; set; }
    public IDictionary<ImperativeType, IEnumerable<string>> ImperativeCases { get; set; }
}