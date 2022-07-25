using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Application.Abstractions;

public interface IStringSimilarityMeasurer
{
    public StringSimilarity MeasureSimilarity(ReadOnlySpan<char> a, ReadOnlySpan<char> b);
}