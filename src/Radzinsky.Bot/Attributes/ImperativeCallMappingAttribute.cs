using Radzinsky.Bot.Enumerations;

namespace Radzinsky.Bot.Attributes;

public class ImperativeCallMappingAttribute : Attribute
{
    public readonly ImperativeType ImperativeType;

    public ImperativeCallMappingAttribute(ImperativeType imperativeType)
    {
        ImperativeType = imperativeType;
    }
}