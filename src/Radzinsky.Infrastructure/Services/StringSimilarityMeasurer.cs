using Radzinsky.Application.Abstractions;
using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Infrastructure.Services;

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
            < 3.0 => StringSimilarity.High,
            < 3.0 => StringSimilarity.Medium,
            _ => StringSimilarity.Low
        };
    }
}
