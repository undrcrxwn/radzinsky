using Radzinsky.Bot.Enumerations;

namespace Radzinsky.Bot.Abstractions;

public interface IImperativeArgumentParser
{
    public IEnumerable<object>? TryParseArguments(ImperativeType imperativeType, ReadOnlyMemory<char> text);
}