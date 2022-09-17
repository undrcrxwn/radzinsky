using Radzinsky.Application.Models;

namespace Radzinsky.Application.Abstractions;

public interface IResourcesService
{
    public CommandResources? GetCommandResources<TCommand>() where TCommand : ICommand;
    public CommandResources? GetCommandResources(string commandTypeName);
    public CommandResources? GetCommandResourcesByAlias(string alias);
    public BehaviorResources GetBehaviorResources(string behaviorTypeName);
}