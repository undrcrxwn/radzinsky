using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Application.Abstractions;

public interface IStringSimilarityMeasurer
{
    public StringSimilarity MeasureSimilarity(string a, string b);
}