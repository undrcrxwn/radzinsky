using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Behaviors;

public class WrongKeyboardLayoutBehavior : IBehavior
{
    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        if (context.Message.Text == context.Message.NormalizedText)
        {
            await next(context);
            return;
        }

        var template = context.Resources!.GetRandom<string>("ProbablyMeant");
        var response = string.Format(template, context.Message.Sender.FirstName, context.Message.NormalizedText);
        await context.ReplyAsync(response);
    }
}