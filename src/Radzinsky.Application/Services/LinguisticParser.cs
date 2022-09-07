using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Enumerations;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Services;

public class LinguisticParser : ILinguisticParser
{
    private readonly SelfResources _selfResources;
    private readonly IEnumerable<CommandResources> _commandResources;
    private readonly IStringSimilarityMeasurer _similarityMeasurer;

    public LinguisticParser(
        SelfResources selfResources,
        IEnumerable<CommandResources> commandResources,
        IStringSimilarityMeasurer similarityMeasurer)
    {
        _selfResources = selfResources;
        _commandResources = commandResources;
        _similarityMeasurer = similarityMeasurer;
    }

    public CaseParsingResult? TryParseMentionFromBeginning(string text) =>
        TryParseCaseFromBeginning(text, _selfResources.Mentions);

    public CaseParsingResult? TryParseCommandAliasFromBeginning(string text)
    {
        var aliases = _commandResources.SelectMany(x => x.Aliases);
        return TryParseCaseFromBeginning(text, aliases);
    }

    public CaseParsingResult? TryParseCaseFromBeginning(string text, IEnumerable<string> cases)
    {
        var candidates = cases
            .Where(@case => @case.Length <= text.Length);

        if (candidates.Count() == 0)
            return null;
        
        var bestMatch = candidates
            .Select(candidate =>
            {
                var beginning = text[..candidate.Length].ToString();
                var candidateSimilarity = _similarityMeasurer.MeasureSimilarity(candidate, beginning);
                
                return new
                {
                    Candidate = candidate,
                    Similarity = candidateSimilarity,
                    Length = beginning.Length
                };
            })
            .OrderByDescending(x => x.Similarity)
            .ThenByDescending(x => x.Length)
            .First();

        return bestMatch.Similarity >= StringSimilarity.High
            ? new CaseParsingResult(text.AsMemory()[..bestMatch.Length], bestMatch.Candidate)
            : null;
    }
    
    public CaseParsingResult? TryParseCaseFromBeginning111(string text, IEnumerable<string> cases)
    {
        var bestMatch = cases
            .Where(@case => @case.Length <= text.Length)
            .Select(candidate =>
            {
                var beginning = text[..candidate.Length].ToString();
                var candidateSimilarity = _similarityMeasurer.MeasureSimilarity(candidate, beginning);
                return new
                {
                    Candidate = candidate,
                    Similarity = candidateSimilarity,
                    Length = beginning.Length
                };
            })
            .OrderByDescending(x => x.Similarity)
            .ThenByDescending(x => x.Length)
            .First();

        return bestMatch.Similarity >= StringSimilarity.High
            ? new CaseParsingResult(text.AsMemory()[..bestMatch.Length], bestMatch.Candidate)
            : null;
    }
}