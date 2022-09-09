using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Commands;

public class MentionCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken) =>
        await context.ReplyAsync(context.Resources.Variants["AtYourService"].PickRandom());
}