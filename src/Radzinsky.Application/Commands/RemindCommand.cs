using Hangfire;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Jobs;
using Radzinsky.Application.Models;
namespace Radzinsky.Application.Commands;

public class RemindCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        BackgroundJob.Schedule<RemindJob>(job =>
            job.RemindAsync(
                context.Message.Chat.Id,
                context.Message.Sender.Id,
                context.Message.Sender.FirstName,
                context.Payload), TimeSpan.FromSeconds(10));

        await context.ReplyAsync(context.Resources.Variants["ReminderSet"].PickRandom());
    }
}