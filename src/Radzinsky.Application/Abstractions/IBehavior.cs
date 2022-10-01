using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Abstractions;

public interface IBehavior
{
    public Task HandleAsync(BehaviorContext context, BehaviorContextHandler next);
}