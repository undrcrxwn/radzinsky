using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Services;

public class ResourcesService : IResourcesService
{
    private readonly IEnumerable<BehaviorResources> _behaviorResources;
    private readonly IEnumerable<CommandResources> _commandResources;

    public ResourcesService(
        IEnumerable<BehaviorResources> behaviorResources,
        IEnumerable<CommandResources> commandResources)
    {
        _behaviorResources = behaviorResources;
        _commandResources = commandResources;
    }

    public CommandResources? GetCommandResources<TCommand>() where TCommand : ICommand =>
        GetCommandResources(typeof(TCommand).FullName);

    public CommandResources? GetCommandResources(string commandTypeName) =>
        _commandResources.FirstOrDefault(x => x.CommandTypeName == commandTypeName);

    public CommandResources? GetCommandResourcesByAlias(string alias) =>
        _commandResources.FirstOrDefault(x => x.Aliases.Contains(alias));

    public BehaviorResources? GetBehaviorResources(string behaviorTypeName) =>
        _behaviorResources.FirstOrDefault(x => x.BehaviorTypeName == behaviorTypeName);
}