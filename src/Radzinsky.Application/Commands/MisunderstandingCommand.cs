using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Commands;

public class MisunderstandingCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context) =>
        await context.ReplyAsync(context.Resources.Variants["CannotUnderstandYou"].PickRandom());
}