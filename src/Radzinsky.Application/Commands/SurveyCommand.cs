using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace Radzinsky.Application.Commands;

public class SurveyCommand : ICommand, ICallbackQueryHandler
{
    private record SurveyState(
        int? MatrixCellId = null,
        int? Rating = null);

    private readonly IStateService _states;
    private readonly IHashingService _hasher;

    public SurveyCommand(IStateService states, IHashingService hasher)
    {
        _states = states;
        _hasher = hasher;
    }

    public async Task HandleCallbackQueryAsync(CallbackQueryContext context, CancellationToken token)
    {
        ParseCallbackData(context.Query.Data, out var respondentUserId, out var callbackKey, out _);

        if (respondentUserId != context.Update.InteractorUserId!.Value)
        {
            await context.ReplyAsync("This survey is not for you!");
            return;
        }

        CallbackQueryContextHandler callbackQueryHandler = callbackKey switch
        {
            "Cell" => HandleMatrixCellCallbackAsync,
            "Rating" => HandleRatingCallbackAsync,
            _ => throw new InvalidOperationException()
        };

        await callbackQueryHandler(context);
    }

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var respondentUserId = context.Update.InteractorUserId!.Value;
        var stateKey = GetSurveyStateKey(respondentUserId);

        if (await _states.ReadStateAsync<SurveyState>(stateKey) is not null)
        {
            await context.ReplyAsync("You are already participating in this survey.");
            return;
        }

        await _states.WriteStateAsync(stateKey, new SurveyState());

        await context.ReplyAsync("Ok! Now answer some questions for the survey.");
        await AskForMatrixCellAsync(context, respondentUserId);
    }

    private async Task AskForMatrixCellAsync(CommandContext context, long respondentUserId)
    {
        var factory = new ButtonFactory(_hasher, nameof(SurveyCommand), "Cell {0}");

        var buttons = new List<List<InlineKeyboardButton>>
        {
            new()
            {
                factory.CreateCallbackDataButton("Cell A", "11"),
                factory.CreateCallbackDataButton("Cell B", "12")
            },
            new()
            {
                factory.CreateCallbackDataButton("Cell C", "21"),
                factory.CreateCallbackDataButton("Cell D", "22")
            }
        };

        await context.ReplyAsync("1. Choose a random matrix cell.",
            replyMarkup: new InlineKeyboardMarkup(buttons));
    }

    private async Task HandleMatrixCellCallbackAsync(CallbackQueryContext context)
    {
        ParseCallbackData(context.Query.Data, out var respondentUserId, out _, out var payload);

        var stateKey = GetSurveyStateKey(respondentUserId);
        var state = await _states.ReadStateAsync<SurveyState>(stateKey);

        if (state!.MatrixCellId is not null)
        {
            await context.ReplyAsync("You've already decided on your matrix cell!");
            return;
        }

        await _states.WriteStateAsync(stateKey, state with { MatrixCellId = int.Parse(payload) });

        await AskForRatingAsync(context, respondentUserId);
    }

    private async Task AskForRatingAsync(CallbackQueryContext context, long respondentUserId)
    {
        var factory = new ButtonFactory(_hasher, nameof(SurveyCommand), "Rating {0}");

        var buttons = new List<List<InlineKeyboardButton>>
        {
            Enumerable.Range(1, 5)
                .Select(x => new
                {
                    Label = new string('*', x),
                    Data = x.ToString()
                })
                .Select(x => factory.CreateCallbackDataButton(x.Label, x.Data))
                .ToList()
        };

        await context.ReplyAsync("2. Rate us 1 to 5.", replyMarkup: new InlineKeyboardMarkup(buttons));
    }

    private async Task HandleRatingCallbackAsync(CallbackQueryContext context)
    {
        ParseCallbackData(context.Query.Data, out var respondentUserId, out _, out var payload);

        var stateKey = GetSurveyStateKey(respondentUserId);
        var state = await _states.ReadStateAsync<SurveyState>(stateKey)!;

        if (state!.Rating is not null)
        {
            await context.ReplyAsync("You've already decided on rating!");
            return;
        }

        await _states.WriteStateAsync(stateKey, state with { Rating = int.Parse(payload) });

        await ShowResultsAsync(context, state);
    }

    private async Task ShowResultsAsync(CallbackQueryContext context, SurveyState state)
    {
        const string replyTemplate = "Thanks for your time! You've just chosen cell {0} and rated us for {1}.";
        await context.ReplyAsync(string.Format(replyTemplate, state.MatrixCellId, state.Rating));
    }

    private static void ParseCallbackData(
        string data,
        out long respondentUserId,
        out string callbackKey,
        out string payload)
    {
        var words = data.Split();
        respondentUserId = long.Parse(words[0]);
        callbackKey = words[1];
        payload = string.Join(' ', words[2..]);
    }

    private string GetSurveyStateKey(long respondentUserId) =>
        $"__InlineSurveyTest__{respondentUserId}";
}