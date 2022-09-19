using Newtonsoft.Json.Linq;

namespace Radzinsky.Application.Models.Resources;

public class CommonResources : Resources
{
    public CommonResources(JObject data)
        : base(data) { }
}