using System.Reflection;
using System.Text.RegularExpressions;

namespace Radzinsky.Framework.Routing.RegEx;

public class RegExEndpointDiscovery
{
    public record EndpointDescriptor(Type EndpointType, IReadOnlyCollection<Regex> Patterns);

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
                Patterns: type.GetCustomAttributes<RegExPatternsAttribute>()
                    .SelectMany(attribute => attribute.Patterns).ToArray()))
            .UnionBy(_endpoints, descriptor => descriptor.EndpointType)
            .ToArray();
    }
}