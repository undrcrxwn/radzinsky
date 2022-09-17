using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Commands;

public class EchoCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken) =>
        await context.ReplyAsync(string.IsNullOrWhiteSpace(context.Payload)
            ? context.Resources.Variants["NoPayload"].PickRandom()
            : context.Payload);
}