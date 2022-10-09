using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot.Types.ReplyMarkups;

namespace Radzinsky.Application.Commands;

public class InlineKeyboardCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var buttons = new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("11", "payload 11"),
                InlineKeyboardButton.WithCallbackData("12", "payload 12"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("21", "payload 21"),
                InlineKeyboardButton.WithCallbackData("22", "payload 22"),
            }
        };

        await context.ReplyAsync("test", replyMarkup: new InlineKeyboardMarkup(buttons));
    }
}