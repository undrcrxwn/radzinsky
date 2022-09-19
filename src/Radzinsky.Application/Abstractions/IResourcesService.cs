using Radzinsky.Application.Models;
using Radzinsky.Application.Models.Resources;

namespace Radzinsky.Application.Abstractions;

public interface IResourcesService
{
    public CommandResources? GetCommandResources<TCommand>() where TCommand : ICommand;
    public CommandResources? GetCommandResources(string commandTypeName);
    public BehaviorResources GetBehaviorResources(string behaviorTypeName);
}