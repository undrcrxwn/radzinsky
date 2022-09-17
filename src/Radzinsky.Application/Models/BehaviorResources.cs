namespace Radzinsky.Application.Models;

public class BehaviorResources
{
    public string BehaviorTypeName;
    public IDictionary<string, IEnumerable<string>> Variants =
        new Dictionary<string, IEnumerable<string>>();
}