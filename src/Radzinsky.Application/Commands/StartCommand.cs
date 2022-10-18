using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Commands;

public class StartCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var response = context.Resources!.Get("Greeting", context.Message.Sender.FirstName);
        await context.DeletePreviousReplyAsync();
        await context.SendTextAsync(response, ParseMode.Html);
    }
}