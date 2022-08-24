using Radzinsky.Bot.Enumerations;

namespace Radzinsky.Bot.Abstractions;

public interface IStringSimilarityMeasurer
{
    public StringSimilarity MeasureSimilarity(string a, string b);
}