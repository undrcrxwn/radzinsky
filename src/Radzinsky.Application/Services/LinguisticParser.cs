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
        if (cases.Count() == 0)
            return null;
        
        var bestMatch = cases
            .Select(candidate =>
            {
                var wordCount = candidate.Split().Length;
                var takenWords = text.Split().Take(wordCount);
                var input = string.Join(' ', takenWords);
                var candidateSimilarity = _similarityMeasurer.MeasureSimilarity(candidate, input);
                
                return new
                {
                    Candidate = candidate,
                    Similarity = candidateSimilarity,
                    Length = input.Length
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