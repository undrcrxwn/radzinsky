using System.Reflection;

namespace Radzinsky.Framework.Routing.StringDistance;

public class StringDistanceEndpointDiscovery
{
    public record EndpointDescriptor(Type EndpointType, IReadOnlyCollection<(string Value, int WordCount)> Aliases);

    public IReadOnlyCollection<EndpointDescriptor> Endpoints => _endpoints;
    private EndpointDescriptor[] _endpoints = [];

    public void ScanAssemblies(IEnumerable<Assembly> assemblies)
    {
        var endpointTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsAssignableTo(typeof(IEndpoint)));

        _endpoints = endpointTypes  
            .Select(type => new EndpointDescriptor(
                EndpointType: type,
                Aliases: type.GetCustomAttributes<StringDistanceAliasesAttribute>()
                    .SelectMany(attribute => attribute.Aliases)
                    .ToArray()))
            .UnionBy(_endpoints, descriptor => descriptor.EndpointType)
            .ToArray();
    }
}