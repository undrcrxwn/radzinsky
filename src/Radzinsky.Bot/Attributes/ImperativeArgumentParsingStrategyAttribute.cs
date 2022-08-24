using Radzinsky.Bot.Enumerations;

namespace Radzinsky.Bot.Attributes;

public class ImperativeArgumentParsingStrategyAttribute : Attribute
{
    public readonly ImperativeType ImperativeType;

    public ImperativeArgumentParsingStrategyAttribute(ImperativeType imperativeType)
    {
        ImperativeType = imperativeType;
    }
}