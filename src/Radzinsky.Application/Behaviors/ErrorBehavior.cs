﻿using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;
using Serilog;

namespace Radzinsky.Application.Behaviors;

public class ErrorBehavior : IBehavior
{
    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            Log.Error(e, "Behavior pipeline has thrown an exception");
            await context.SendTextAsync(context.Resources!.GetRandom<string>("SomethingWentWrong"));
        }
    }
}