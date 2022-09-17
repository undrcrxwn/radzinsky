using Radzinsky.Application.Models;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Delegates;

public delegate Task BehaviorContextHandler(BehaviorContext context);