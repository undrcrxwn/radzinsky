using MediatR;
using Serilog;

namespace Radzinsky.Application.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        Log.Information("Handling {0}", request);
        var response = await next();
        Log.Information("{0} handled with return of {1}", request, response);
        return response;
    }
}