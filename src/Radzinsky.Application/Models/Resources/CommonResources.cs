using Newtonsoft.Json.Linq;

namespace Radzinsky.Application.Models.Resources;

public class CommonResources : ResourcesBase
{
    public CommonResources(JObject data)
        : base(data) { }
}