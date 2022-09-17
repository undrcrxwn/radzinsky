using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Behaviors;

public class MentionBehavior : IBehavior
{
    private readonly ILinguisticParser _parser;

    public MentionBehavior(ILinguisticParser parser) =>
        _parser = parser;

    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        var mention = _parser.TryParseMentionFromBeginning(context.Message.NormalizedText);
        if (mention is null || !string.IsNullOrWhiteSpace(
                context.Message.Text.Substring(mention.Segment.Length)))
        {
            next(context);
            return;
        }

        context.SetMentionCheckpoint();
        await context.ReplyAsync(context.Resources.Variants["AtYourService"].PickRandom());
    }
}