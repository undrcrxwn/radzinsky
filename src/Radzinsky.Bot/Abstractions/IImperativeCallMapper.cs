using MediatR;
using Radzinsky.Bot.Models;
using Telegram.Bot.Types;

namespace Radzinsky.Bot.Abstractions;

public interface IImperativeCallMapper
{
    public IBaseRequest MapToRequest(ImperativeCall call, Message context);
}