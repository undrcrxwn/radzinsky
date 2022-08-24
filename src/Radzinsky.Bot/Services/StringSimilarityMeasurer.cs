using Radzinsky.Bot.Abstractions;
using Radzinsky.Bot.Enumerations;

namespace Radzinsky.Bot.Services;

public class StringSimilarityMeasurer : IStringSimilarityMeasurer
{
    private const int StringPartLength = 3;
    private readonly IStringDistanceMeasurer _distanceMeasurer;

    public StringSimilarityMeasurer(IStringDistanceMeasurer distanceMeasurer)
    {
        _distanceMeasurer = distanceMeasurer;
    }

    public StringSimilarity MeasureSimilarity(string a, string b)
    {
        var distance = _distanceMeasurer.MeasureDistance(a.ToLower(), b.ToLower());
        var distancePerPart = distance / (double) StringPartLength;
        return distancePerPart switch
        {
            0 => StringSimilarity.Equal,
            < 1.0 => StringSimilarity.High,
            < 2.0 => StringSimilarity.Medium,
            _ => StringSimilarity.Low
        };
    }
}