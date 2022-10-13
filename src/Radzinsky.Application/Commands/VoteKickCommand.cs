using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Commands;

public class VoteKickCommand : ICommand, ICallbackQueryHandler
{
    private record SurveyState(long RespondentUserId, int? MatrixCellId, int? Rating);

    private readonly IHashingService _hasher;
    private readonly dynamic _stateService = null;

    public VoteKickCommand(IHashingService hasher) =>
        _hasher = hasher;

    public Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
    }

    public Task HandleCallbackQueryAsync(CallbackQueryContext context, CancellationToken token)
    {
    }
}