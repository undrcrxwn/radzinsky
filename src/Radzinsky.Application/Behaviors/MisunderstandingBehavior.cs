using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Behaviors;

public class MisunderstandingBehavior : IBehavior
{
    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        await context.ReplyAsync(context.Resources.GetRandom("CannotUnderstandYou"));
        await next(context);
    }
}