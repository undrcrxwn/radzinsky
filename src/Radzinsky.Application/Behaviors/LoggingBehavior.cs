using MediatR;
using Serilog;

namespace Radzinsky.Application.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        TResponse? response = default;
        try
        {
            Log.Debug("Handling {@0}", request);
            response = await next();
            return response;
        }
        finally
        {
            Log.Debug("{0} handled with return of {@1}", request, response);
        }
    }
}