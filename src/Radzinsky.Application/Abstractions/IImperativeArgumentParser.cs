using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Application.Abstractions;

public interface IImperativeArgumentParser
{
    public IEnumerable<object>? TryParseArguments(ImperativeType imperativeType, ReadOnlyMemory<char> text);
}