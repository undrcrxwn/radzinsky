using Newtonsoft.Json.Linq;

namespace Radzinsky.Application.Models.Resources;

public class BehaviorResources : Resources
{
    public BehaviorResources(JObject data)
        : base(data) { }
}