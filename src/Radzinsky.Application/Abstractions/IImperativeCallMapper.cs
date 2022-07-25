using MediatR;
using Radzinsky.Domain.Models;
using Telegram.Bot.Types;

namespace Radzinsky.Application.Abstractions;

public interface IImperativeCallMapper
{
    public IBaseRequest MapToRequest(ImperativeCall call, Message context);
}