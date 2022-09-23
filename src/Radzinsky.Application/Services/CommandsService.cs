using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Resources;

namespace Radzinsky.Application.Services;

public class CommandsService : ICommandsService
{
    private readonly IDictionary<string, CommandResources> _commandResources;

    public CommandsService(IDictionary<string, CommandResources> commandResources) =>
        _commandResources = commandResources;

    public ICommand GetCommandInstance(IServiceScope scope, string commandTypeName)
    {
        var commandType = Type.GetType(commandTypeName);
        return (ICommand)scope.ServiceProvider.GetRequiredService(commandType);
    }

    public string? GetCommandTypeNameByAlias(string alias) =>
        _commandResources.FirstOrDefault(x => x.Value.Aliases.Contains(alias)).Key;

    public string? GetCommandTypeNameBySlash(string slash) =>
        _commandResources.FirstOrDefault(x => x.Value.Slashes.Contains(slash.TrimStart('/'))).Key;
}