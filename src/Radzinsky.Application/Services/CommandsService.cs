using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Services;

public class CommandsService : ICommandsService
{
    private readonly IEnumerable<CommandResources> _commandResources;
    
    public CommandsService(IEnumerable<CommandResources> commandResources) =>
        _commandResources = commandResources;
    
    public CommandResources GetResources<TCommand>() where TCommand : ICommand =>
        GetResources(typeof(TCommand).FullName);

    public CommandResources GetResources(string commandTypeName) =>
        _commandResources.First(x => x.CommandTypeName == commandTypeName);

    public CommandResources GetResourcesByAlias(string alias) =>
        _commandResources.First(x => x.Aliases.Contains(alias));

    public ICommand GetCommandInstance(IServiceScope scope, string commandTypeName)
    {
        var commandType = Type.GetType(commandTypeName);
        return (ICommand)scope.ServiceProvider.GetRequiredService(commandType);
    }
}