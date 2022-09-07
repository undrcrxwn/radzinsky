using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Commands;

public class EchoCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context) =>
        await context.ReplyAsync(context.Payload);
}