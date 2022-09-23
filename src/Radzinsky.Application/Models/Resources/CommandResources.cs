using Newtonsoft.Json.Linq;

namespace Radzinsky.Application.Models.Resources;

public class CommandResources : Resources
{
    public CommandResources(JObject data)
        : base(data) { }

    public IEnumerable<string> Aliases =>
        GetManyOrEmpty<string>("Aliases");
    
    public IEnumerable<string> Slashes =>
        GetManyOrEmpty<string>("Slashes");
}