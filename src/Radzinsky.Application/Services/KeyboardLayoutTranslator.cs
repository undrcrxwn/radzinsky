using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Enumerations;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Services;

public class KeyboardLayoutTranslator : IKeyboardLayoutTranslator
{
    const string RussianCharacters = "ёйцукенгшщзхъфывапролджэячсмитьбюЁЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ";
    const string EnglishCharacters = "`qwertyuiop[]asdfghjkl;'zxcvbnm,.~QWERTYUIOP{}ASDFGHJKL:\"ZXCVBNM<>";
    private const double NaturalnessThreshold = 0.5;
    private const int MinInputLength = 5;

    private readonly IStringSimilarityMeasurer _similarityMeasurer;
    private readonly BigramFrequencies _frequencies;

    private static readonly IDictionary<char, char> EnglishToRussianCharacters =
        MapItems(EnglishCharacters, RussianCharacters);

    public KeyboardLayoutTranslator(
        IStringSimilarityMeasurer similarityMeasurer,
        BigramFrequencies frequencies)
    {
        _similarityMeasurer = similarityMeasurer;
        _frequencies = frequencies;
    }

    public string Translate(string input)
    {
        if (input.Length < MinInputLength)
            return input;

        var russianTranslation = TranslateByDictionary(input, EnglishToRussianCharacters);

        var combinations = new (string Output, IDictionary<string, double> Frequencies)[]
        {
            (input, _frequencies.EnglishBigramFrequencies),
            (input, _frequencies.RussianBigramFrequencies),
            (russianTranslation, _frequencies.RussianBigramFrequencies)
        };

        var scores = combinations.Select(x => new
        {
            Output = x.Output,
            Naturality = MeasureNaturalness(x.Output, x.Frequencies)
        });

        var bestResult = scores.MaxBy(x => x.Naturality);

        return bestResult.Naturality >= NaturalnessThreshold &&
               _similarityMeasurer.MeasureSimilarity(input, bestResult.Output) <= StringSimilarity.Low
            ? bestResult.Output
            : input;
    }

    private double MeasureNaturalness(string text, IDictionary<string, double> frequencies)
    {
        text = text.ToLower();

        var metrics = text
            .Zip(text.Skip(1), (a, b) =>
                frequencies.TryGetValue($"{a}{b}", out var frequency)
                    ? frequency
                    : 0)
            .ToArray();

        return Math.Sqrt(metrics.Sum() / metrics.Length);
    }

    private string TranslateByDictionary(string text, IDictionary<char, char> dictionary) =>
        string.Join(string.Empty, text.Select(originalCharacter =>
        {
            var found = dictionary.TryGetValue(originalCharacter, out var translatedCharacter);
            return found ? translatedCharacter : originalCharacter;
        }));

    private static IDictionary<T1, T2> MapItems<T1, T2>(IEnumerable<T1> a, IEnumerable<T2> b) where T1 : notnull =>
        a.Zip(b).ToDictionary(x => x.First, x => x.Second);
}