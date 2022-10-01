using Hangfire;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Jobs;
using Radzinsky.Application.Models.Contexts;

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
                context.Payload), TimeSpan.FromMinutes(5));

        await context.ReplyAsync(context.Resources!.GetRandom<string>("ReminderSet"));
    }
}