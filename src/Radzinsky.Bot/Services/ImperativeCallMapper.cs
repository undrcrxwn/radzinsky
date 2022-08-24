using System.Reflection;
using MediatR;
using Radzinsky.Bot.Abstractions;
using Radzinsky.Bot.Attributes;
using Radzinsky.Bot.Enumerations;
using Radzinsky.Bot.Models;
using Telegram.Bot.Types;

namespace Radzinsky.Bot.Services;

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