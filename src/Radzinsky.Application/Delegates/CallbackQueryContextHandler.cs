using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Delegates;

public delegate Task CallbackQueryContextHandler(CallbackQueryContext context);