namespace Radzinsky.Application.Models;

public class CommandResources
{
    public string CommandTypeName;
    public IEnumerable<string> Aliases = Enumerable.Empty<string>();
    public IDictionary<string, IEnumerable<string>> Variants =
        new Dictionary<string, IEnumerable<string>>();
}