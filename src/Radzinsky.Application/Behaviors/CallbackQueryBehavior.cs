using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Behaviors;

public class CallbackQueryBehavior : IBehavior
{
    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        if (context.Update.Type != UpdateType.CallbackQuery)
        {
            await next(context);
            return;
        }

        if (context.GetCheckpoint() is CallbackQueryWaitingCheckpoint checkpoint)
        {
            checkpoint.
        }
    }
}