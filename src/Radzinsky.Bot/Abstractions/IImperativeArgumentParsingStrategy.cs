namespace Radzinsky.Bot.Abstractions;

public interface IImperativeArgumentParsingStrategy
{
    public IEnumerable<object>? TryParseArguments(ReadOnlyMemory<char> text);
}