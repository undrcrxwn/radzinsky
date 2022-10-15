using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Behaviors;

public class MisunderstandingBehavior : IBehavior
{
    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        if (context.Update.Message!.IsReplyToMe)
        {
            await next(context);
            return;
        }
        
        var checkpoint =  context.GetCheckpoint();
        var isBotMentioned = checkpoint is { Name: "BotMentioned" };
        
        if (context.Update.Message.IsPrivate ||
            context.Update.Message.IsReplyToMe ||
            context.Update.Message.StartsWithMyName ||
            isBotMentioned)
        {
            if (isBotMentioned)
                context.ResetCheckpoint();
            
            await context.ReplyAsync(context.Resources!.GetRandom("CannotUnderstandYou"));
            return;
        }

        await next(context);
    }
}