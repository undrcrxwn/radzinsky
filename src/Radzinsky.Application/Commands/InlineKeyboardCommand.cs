using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot.Types.ReplyMarkups;

namespace Radzinsky.Application.Commands;

public class InlineKeyboardCommand : ICommand
{
    private record SurveyPayload(string? Cell, string? Rating);

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        Func<CommandContext, Task> strategy = context.GetLocalCheckpoint() switch
        {
            { Name: "AskedForMatrixCell" } => AskForRating,
            { Name: "AskedForRating" } => ShowResults,
            _ => AskForMatrixCell
        };

        await strategy();
    }

    private async Task AskForMatrixCell(CommandContext context)
    {
        var buttons = new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Cell #11", "11"),
                InlineKeyboardButton.WithCallbackData("Cell #12", "12"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Cell #21", "21"),
                InlineKeyboardButton.WithCallbackData("Cell #22", "22"),
            }
        };

        await context.ReplyAsync("Choose a matrix cell", replyMarkup: new InlineKeyboardMarkup(buttons));
        context.SetCheckpoint(new SurveyPayload("AskedForCell"));
    }

    private async Task AskForRating(CommandContext context)
    {
        var buttons = new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("*", "1 start"),
                InlineKeyboardButton.WithCallbackData("**", "2 starts"),
                InlineKeyboardButton.WithCallbackData("***", "3 starts"),
                InlineKeyboardButton.WithCallbackData("****", "4 starts"),
                InlineKeyboardButton.WithCallbackData("*****", "5 starts")
            }
        };

        await context.ReplyAsync("Rate us", replyMarkup: new InlineKeyboardMarkup(buttons));
        context.SetCheckpoint("AskedForRating");
    }
    
    private async Task ShowResults(CommandContext context)
    {
        var checkpoint = context.GetLocalCheckpoint<(int, int)>();
        var replyTemplate = "You have chosen cell {0} and rated us for {1}";
        await context.ReplyAsync(string.Format(replyTemplate, cellPayload, ratingPayload));
    }
}