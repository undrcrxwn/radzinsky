using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Behaviors;

public class MisunderstandingBehavior : IBehavior
{
    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        if (context.Message.IsReplyToMe)
        {
            await next(context);
            return;
        }
        
        if (context.Message.IsPrivate ||
            context.Message.StartsWithMyName ||
            context.Checkpoint is MentionCheckpoint)
        {
            if (context.Checkpoint is MentionCheckpoint)
                context.ResetCheckpoint();
            
            await context.ReplyAsync(context.Resources!.GetRandom("CannotUnderstandYou"));
            return;
        }

        await next(context);
    }
}