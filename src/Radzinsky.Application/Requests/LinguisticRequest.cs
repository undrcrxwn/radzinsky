using MediatR;
using Radzinsky.Application.Abstractions;
using Telegram.Bot.Types;

namespace Radzinsky.Application.Requests;

public record LinguisticRequest(
        string Text, bool IsDirectMessage, Message Context)
    : IRequest;

internal class LinguisticRequestHandler : IRequestHandler<LinguisticRequest>
{
    private readonly IMediator _mediator;
    private readonly ILinguisticParser _parser;
    private readonly IImperativeCallMapper _imperativeCallMapper;

    public LinguisticRequestHandler(
        IMediator mediator,
        ILinguisticParser parser,
        IImperativeCallMapper imperativeCallMapper)
    {
        _mediator = mediator;
        _parser = parser;
        _imperativeCallMapper = imperativeCallMapper;
    }

    public async Task<Unit> Handle(LinguisticRequest request, CancellationToken cancellationToken)
    {
        var text = request.Text.AsMemory();

        // Require mention for non-direct messages (e.g. groups)
        var parsedMention = _parser.TryParseMentionCaseFromBeginning(text);
        if (!request.IsDirectMessage && parsedMention is null)
            return Unit.Value;

        // Remove mention from beginning of the text
        if (parsedMention is not null)
            text = text[parsedMention.Segment.Length..];

        // Form imperative request
        var imperativeCall = _parser.TryParseImperativeCall(text);
        var imperativeRequest = imperativeCall is not null
            ? _imperativeCallMapper.MapToRequest(imperativeCall, request.Context)
            : new MentionRequest(request.Context.Chat.Id);

        // Execute imperative request
        await _mediator.Send(imperativeRequest);
        return Unit.Value;
    }
}