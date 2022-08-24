namespace Radzinsky.Bot.Abstractions;

public interface IArgumentParsingStrategy
{
    public IEnumerable<object> ParseArguments(ReadOnlyMemory<char> text);
}