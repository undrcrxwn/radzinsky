using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;

namespace Radzinsky.Application.Services;

public class CommandsService : ICommandsService
{
    public ICommand GetCommandInstance(IServiceScope scope, string commandTypeName)
    {
        var commandType = Type.GetType(commandTypeName);
        return (ICommand)scope.ServiceProvider.GetRequiredService(commandType);
    }
}