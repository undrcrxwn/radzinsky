using MediatR;
using Telegram.Bot.Types;

namespace Radzinsky.Bot.Abstractions;

public interface IImperativeCallMapping
{
    public IBaseRequest MapToRequest(Message context, IEnumerable<object> arguments);
}