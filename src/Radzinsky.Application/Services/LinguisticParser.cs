using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Enumerations;
using Radzinsky.Application.Models;
using Radzinsky.Application.Models.Resources;

namespace Radzinsky.Application.Services;

public class LinguisticParser : ILinguisticParser
{
    private readonly CommonResources _commonResources;
    private readonly IEnumerable<CommandResources> _commandResources;
    private readonly IStringSimilarityMeasurer _similarityMeasurer;

    public LinguisticParser(
        CommonResources commonResources,
        IEnumerable<CommandResources> commandResources,
        IStringSimilarityMeasurer similarityMeasurer)
    {
        _commonResources = commonResources;
        _commandResources = commandResources;
        _similarityMeasurer = similarityMeasurer;
    }

    public CaseParsingResult? TryParseMentionFromBeginning(string text) =>
        TryParseCaseFromBeginning(text, _commonResources.Mentions);

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