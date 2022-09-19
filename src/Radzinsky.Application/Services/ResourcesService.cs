using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;
using Radzinsky.Application.Models.Resources;

namespace Radzinsky.Application.Services;

public class ResourcesService : IResourcesService
{
    private readonly IDictionary<string, BehaviorResources> _behaviorResources;
    private readonly IDictionary<string, CommandResources> _commandResources;

    public ResourcesService(
        IDictionary<string, BehaviorResources> behaviorResources,
        IDictionary<string, CommandResources> commandResources)
    {
        _behaviorResources = behaviorResources;
        _commandResources = commandResources;
    }

    public CommandResources? GetCommandResources<TCommand>() where TCommand : ICommand =>
        GetCommandResources(typeof(TCommand).FullName);

    public CommandResources? GetCommandResources(string commandTypeName)
    {
        var found = _commandResources.TryGetValue(commandTypeName, out var resources);
        return found ? resources : null;
    }

    public BehaviorResources? GetBehaviorResources(string behaviorTypeName)
    {
        var found = _behaviorResources.TryGetValue(behaviorTypeName, out var resources);
        return found ? resources : null;
    }
}