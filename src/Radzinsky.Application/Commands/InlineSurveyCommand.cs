using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot.Types.ReplyMarkups;

namespace Radzinsky.Application.Commands;

public class InlineSurveyCommand : ICommand, ICallbackQueryHandler
{
    private readonly IHashingService _hasher;

    private Dictionary<string, Func<CallbackQueryContext, Task>> CallbackQueryHandlerMap;

    public InlineSurveyCommand(IHashingService hasher)
    {
        _hasher = hasher;
        
        CallbackQueryHandlerMap = new()
        {
            [_hasher.HashKey(nameof(AskForMatrixCellAsync))] = AskForRatingAsync,
            [_hasher.HashKey(nameof(AskForRatingAsync))] = ShowResultsAsync
        };
    }

    public async Task HandleCallbackQueryAsync(CallbackQueryContext context, CancellationToken token) =>
        await CallbackQueryHandlerMap[context.Query.Data.HashedIssuerKey](context);

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        await context.ReplyAsync("Ok! Now answer some questions for the survey.");
        await AskForMatrixCellAsync(context);
    }

    private async Task AskForMatrixCellAsync(CommandContext context)
    {
        var buttonFactory = new IssuedCallbackButtonFactory(nameof(AskForMatrixCellAsync));
        
        var buttons = new[]
        {
            new[]
            {
                buttonFactory.CreateInlineKeyboardButton("Cell A", "11"),
                buttonFactory.CreateInlineKeyboardButton("Cell B", "12")
            },
            new[]
            {
                buttonFactory.CreateInlineKeyboardButton("Cell C", "21"),
                buttonFactory.CreateInlineKeyboardButton("Cell D", "22"),
            }
        };

        await context.ReplyAsync("1. Choose a random matrix cell.", replyMarkup: new InlineKeyboardMarkup(buttons));
    }

    private async Task AskForRatingAsync(CallbackQueryContext context)
    {
        var matrixCell = int.Parse(context.Query.Data.Payload);
        
        var buttons = new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("*", "1"),
                InlineKeyboardButton.WithCallbackData("**", "2"),
                InlineKeyboardButton.WithCallbackData("***", "3"),
                InlineKeyboardButton.WithCallbackData("****", "4"),
                InlineKeyboardButton.WithCallbackData("*****", "5")
            }
        };

        await context.ReplyAsync("2. Rate us 1 to 5.", replyMarkup: new InlineKeyboardMarkup(buttons));
        context.SetCheckpoint("AskedForRating", matrixCell);
    }
    
    private async Task ShowResultsAsync(CallbackQueryContext context)
    {
        var matrixCell = context.GetLocalCheckpoint<int>()!.Payload;
        var rating = int.Parse(context.Query.Data.Payload);
        
        var replyTemplate = "Thanks for your time! You've just chosen cell {0} and rated us for {1}.";
        await context.ReplyAsync(string.Format(replyTemplate, matrixCell, rating));
    }
}