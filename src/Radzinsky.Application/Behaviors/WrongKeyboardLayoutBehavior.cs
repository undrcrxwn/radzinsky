using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Behaviors;

public class WrongKeyboardLayoutBehavior : IBehavior
{
    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        if (context.Update.Message!.Text == context.Update.Message.NormalizedText)
        {
            await next(context);
            return;
        }

        var format = context.Resources!.GetRandom<string>("ProbablyMeant");
        var response = string.Format(format, context.Update.Message.Sender.FirstName, context.Update.Message.NormalizedText);
        await context.ReplyAsync(response);
    }
}