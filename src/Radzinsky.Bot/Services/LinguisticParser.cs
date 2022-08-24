using Radzinsky.Bot.Abstractions;
using Radzinsky.Bot.Enumerations;
using Radzinsky.Bot.Models;

namespace Radzinsky.Bot.Services;

public class LinguisticParser : ILinguisticParser
{
    private readonly Resources _resources;
    private readonly IStringSimilarityMeasurer _similarityMeasurer;
    private readonly IImperativeArgumentParser _argumentParser;

    public LinguisticParser(
        Resources resources,
        IStringSimilarityMeasurer similarityMeasurer,
        IImperativeArgumentParser argumentParser)
    {
        _resources = resources;
        _similarityMeasurer = similarityMeasurer;
        _argumentParser = argumentParser;
    }

    public CaseParsingResult? TryParseMentionCaseFromBeginning(ReadOnlyMemory<char> text)
    {
        return TryParseCaseFromBeginning(text, _resources.PlainTextCases[PlainTextType.Mention]);
    }

    public ImperativeCall? TryParseImperativeCall(ReadOnlyMemory<char> text)
    {
        var parsedImperative = TryParseImperativeCaseFromBeginning(text);
        if (parsedImperative is null)
            return null;

        var argumentsSegment = text[parsedImperative.Segment.Length..];
        var arguments = TryParseImperativeArguments(argumentsSegment, parsedImperative.Value);
        return arguments is not null
            ? new ImperativeCall(parsedImperative.Value, arguments)
            : null;
    }

    public CaseParsingResult<ImperativeType>? TryParseImperativeCaseFromBeginning(ReadOnlyMemory<char> text)
    {
        var imperativeCases = _resources.ImperativeCases.SelectMany(x => x.Value);

        var result = TryParseCaseFromBeginning(text, imperativeCases);
        if (result is null)
            return null;

        var imperativeType = _resources.ImperativeCases
            .First(cases => cases.Value.Contains(result.Case))
            .Key;

        return new CaseParsingResult<ImperativeType>(result.Segment, result.Case, imperativeType);
    }

    public IEnumerable<object>? TryParseImperativeArguments(ReadOnlyMemory<char> text, ImperativeType imperativeType)
    {
        return _argumentParser.TryParseArguments(imperativeType, text);
    }

    public CaseParsingResult? TryParseCaseFromBeginning(ReadOnlyMemory<char> text, IEnumerable<string> cases)
    {
        var (@case, similarity, length) = cases
            .Where(@case => @case.Length <= text.Length)
            .Select(candidate =>
            {
                var beginning = text[..candidate.Length].ToString();
                var candidateSimilarity = _similarityMeasurer.MeasureSimilarity(candidate, beginning);
                return (candidate, Similarity: candidateSimilarity, beginning.Length);
            })
            .OrderByDescending(x => x.Similarity)
            .ThenByDescending(x => x.Length)
            .First();

        return similarity >= StringSimilarity.High
            ? new CaseParsingResult(text[..length], @case)
            : null;
    }
}