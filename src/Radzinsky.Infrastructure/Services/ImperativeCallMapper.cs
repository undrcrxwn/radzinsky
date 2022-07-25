using System.Reflection;
using MediatR;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Attributes;
using Radzinsky.Domain.Enumerations;
using Radzinsky.Domain.Models;
using Telegram.Bot.Types;

namespace Radzinsky.Infrastructure.Services;

public class ImperativeCallMapper : IImperativeCallMapper
{
    private readonly IDictionary<ImperativeType, IImperativeCallMapping> _mappings;

    public ImperativeCallMapper(IEnumerable<IImperativeCallMapping> mappings)
    {
        _mappings = mappings
            .ToDictionary(
                mapping =>
                {
                    var attribute = mapping.GetType().GetCustomAttribute<ImperativeCallMappingAttribute>();
                    if (attribute is null)
                        throw new InvalidOperationException(
                            $"Imperative call mapping must have {nameof(ImperativeCallMappingAttribute)}");
                    return attribute.ImperativeType;
                },
                mapping => mapping);
    }

    public IBaseRequest MapToRequest(ImperativeCall call, Message context)
    {
        return _mappings[call.ImperativeType].MapToRequest(context, call.Arguments);
    }
}