using System.Reflection;
using Radzinsky.Bot.Abstractions;
using Radzinsky.Bot.Attributes;
using Radzinsky.Bot.Enumerations;

namespace Radzinsky.Bot.Services;

public class ImperativeArgumentParser : IImperativeArgumentParser
{
    private readonly IDictionary<ImperativeType, IImperativeArgumentParsingStrategy> _strategies;

    public ImperativeArgumentParser(IEnumerable<IImperativeArgumentParsingStrategy> strategies)
    {
        _strategies = strategies
            .ToDictionary(
                strategy =>
                {
                    var attribute = strategy.GetType().GetCustomAttribute<ImperativeArgumentParsingStrategyAttribute>();
                    if (attribute is null)
                        throw new InvalidOperationException(
                            $"Imperative argument parsing strategy must have {nameof(ImperativeArgumentParsingStrategyAttribute)}");
                    return attribute.ImperativeType;
                },
                strategy => strategy);
    }

    public IEnumerable<object>? TryParseArguments(ImperativeType imperativeType, ReadOnlyMemory<char> text)
    {
        return _strategies[imperativeType].TryParseArguments(text);
    }
}