namespace Radzinsky.Application.Models.Resources;

public class BehaviorResources
{
    public string BehaviorTypeName;
    public IDictionary<string, IEnumerable<string>> Variants =
        new Dictionary<string, IEnumerable<string>>();
}