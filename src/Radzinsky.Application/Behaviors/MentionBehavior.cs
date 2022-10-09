using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Behaviors;

public class MentionBehavior : IBehavior
{
    private readonly ILinguisticParser _parser;
    private readonly ICheckpointMemoryService _checkpoints;
    
    public MentionBehavior(ILinguisticParser parser) =>
        _parser = parser;

    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        if (context.Update.Type != UpdateType.Message)
        {
            await next(context);
            return;
        }
        
        var mention = _parser.TryParseMentionFromBeginning(context.Update.Message!.NormalizedText);
        if (mention is null || !string.IsNullOrWhiteSpace(
                context.Update.Message.Text.Substring(mention.Segment.Length)))
        {
            await next(context);
            return;
        }

        context.SetCheckpoint(CommandCheckpoint.From with { Name = "BotMentioned" });
        await context.ReplyAsync(context.Resources!.GetRandom<string>("AtYourService"));
    }
}