using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Domain.Models;

public class Resources
{
    public IDictionary<PlainTextType, IEnumerable<string>> PlainTextCases { get; set; }
    public IDictionary<ImperativeType, IEnumerable<string>> ImperativeCases { get; set; }
}