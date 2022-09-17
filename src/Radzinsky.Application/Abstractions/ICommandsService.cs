using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Abstractions;

public interface ICommandsService
{
    public ICommand GetCommandInstance(IServiceScope scope, string commandTypeName);
}