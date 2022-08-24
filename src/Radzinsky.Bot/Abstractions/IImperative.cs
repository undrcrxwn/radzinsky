using MediatR;

namespace Radzinsky.Bot.Abstractions;

public interface IImperative<in TRequest> :
    IImperativeArgumentParsingStrategy,
    IImperativeCallMapping,
    IRequestHandler<TRequest>
    where TRequest : IRequest<Unit>
{
}