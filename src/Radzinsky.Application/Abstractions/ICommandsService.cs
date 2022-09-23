using Microsoft.Extensions.DependencyInjection;

namespace Radzinsky.Application.Abstractions;

public interface ICommandsService
{
    public ICommand GetCommandInstance(IServiceScope scope, string commandTypeName);
    public string? GetCommandTypeNameByAlias(string alias);
    public string? GetCommandTypeNameBySlash(string slash);
}