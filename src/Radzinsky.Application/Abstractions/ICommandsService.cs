using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Abstractions;

public interface ICommandsService
{
    public CommandResources GetResources<TCommand>() where TCommand : ICommand;
    public CommandResources GetResources(string commandTypeName);
    public CommandResources GetResourcesByAlias(string alias);
    public ICommand GetCommandInstance(IServiceScope scope, string commandTypeName);
}