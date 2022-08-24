namespace Radzinsky.Bot.Models;

public record CaseParsingResult(ReadOnlyMemory<char> Segment, string Case);

public record CaseParsingResult<T>(ReadOnlyMemory<char> Segment, string Case, T Value)
    : CaseParsingResult(Segment, Case);