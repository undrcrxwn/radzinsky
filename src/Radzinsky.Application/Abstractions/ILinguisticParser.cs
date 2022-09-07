using Radzinsky.Application.Models;

namespace Radzinsky.Application.Abstractions;

public interface ILinguisticParser
{
    public CaseParsingResult? TryParseMentionFromBeginning(string text);
    public CaseParsingResult? TryParseCommandAliasFromBeginning(string text);
    public CaseParsingResult? TryParseCaseFromBeginning(string text, IEnumerable<string> cases);
}