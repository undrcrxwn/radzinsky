using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Commands;

public class PortalCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        await context.SendTextAsync(context.Resources!.GetRandom("Searching"));
    }
}