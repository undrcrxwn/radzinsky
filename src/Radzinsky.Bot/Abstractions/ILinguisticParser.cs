using Radzinsky.Bot.Enumerations;
using Radzinsky.Bot.Models;

namespace Radzinsky.Bot.Abstractions;

public interface ILinguisticParser
{
    public CaseParsingResult? TryParseMentionCaseFromBeginning(ReadOnlyMemory<char> text);
    public ImperativeCall? TryParseImperativeCall(ReadOnlyMemory<char> text);
    public CaseParsingResult<ImperativeType>? TryParseImperativeCaseFromBeginning(ReadOnlyMemory<char> text);
    public IEnumerable<object>? TryParseImperativeArguments(ReadOnlyMemory<char> text, ImperativeType imperativeType);
    public CaseParsingResult? TryParseCaseFromBeginning(ReadOnlyMemory<char> text, IEnumerable<string> cases);
}