using MediatR;

namespace Radzinsky.Application.Abstractions;

public interface IImperative<in TRequest> :
    IImperativeArgumentParsingStrategy,
    IImperativeCallMapping,
    IRequestHandler<TRequest>
    where TRequest : IRequest<Unit>
{
}