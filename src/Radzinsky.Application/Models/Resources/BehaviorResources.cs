using Newtonsoft.Json.Linq;

namespace Radzinsky.Application.Models.Resources;

public class BehaviorResources : ResourcesBase
{
    public BehaviorResources(JObject data)
        : base(data) { }
}