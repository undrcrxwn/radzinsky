using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Application.Attributes;

public class ImperativeArgumentParsingStrategyAttribute : Attribute
{
    public readonly ImperativeType ImperativeType;

    public ImperativeArgumentParsingStrategyAttribute(ImperativeType imperativeType)
    {
        ImperativeType = imperativeType;
    }
}