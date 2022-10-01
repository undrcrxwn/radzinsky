using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Abstractions;

public interface ICommand
{
    public Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken);
}