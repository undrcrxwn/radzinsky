using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Application.Attributes;

public class ImperativeCallMappingAttribute : Attribute
{
    public readonly ImperativeType ImperativeType;

    public ImperativeCallMappingAttribute(ImperativeType imperativeType)
    {
        ImperativeType = imperativeType;
    }
}