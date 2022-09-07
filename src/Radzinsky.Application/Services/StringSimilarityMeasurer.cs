using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Enumerations;

namespace Radzinsky.Application.Services;

public class StringSimilarityMeasurer : IStringSimilarityMeasurer
{
    private readonly IStringDistanceMeasurer _distanceMeasurer;

    public StringSimilarityMeasurer(IStringDistanceMeasurer distanceMeasurer) =>
        _distanceMeasurer = distanceMeasurer;

    public StringSimilarity MeasureSimilarity(string a, string b)
    {
        var distance = _distanceMeasurer.MeasureDistance(a.ToLower(), b.ToLower());
        var averageInputLength = (a.Length + b.Length) / 2.0;
        var distancePerCharacter = distance / averageInputLength;

        return distancePerCharacter switch
        {
            0 => StringSimilarity.Equal,
            < 0.4 => StringSimilarity.High,
            < 0.7 => StringSimilarity.Medium,
            _ => StringSimilarity.Low
        };
    }
}