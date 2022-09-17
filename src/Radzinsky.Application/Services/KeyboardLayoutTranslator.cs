using System.Globalization;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Services;

public class KeyboardLayoutTranslator : IKeyboardLayoutTranslator
{
    const string RussianCharacters = "йцукенгшщзхъфывапролджэячсмитьбюЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ";
    const string EnglishCharacters = "qwertyuiop[]asdfghjkl;'zxcvbnm,.QWERTYUIOP{}ASDFGHJKL:\"ZXCVBNM<>";
    private const double NaturalnessThreshold = 0.5;

    private readonly BigramFrequencies _frequencies;

    private static readonly IDictionary<char, char> RussianToEnglishCharacters =
        MapItems(RussianCharacters, EnglishCharacters);

    private static readonly IDictionary<char, char> EnglishToRussianCharacters =
        MapItems(EnglishCharacters, RussianCharacters);

    public KeyboardLayoutTranslator(BigramFrequencies frequencies) =>
        _frequencies = frequencies;

    public string Translate(string input)
    {
        var englishTranslation = TranslateByDictionary(input, RussianToEnglishCharacters);
        var russianTranslation = TranslateByDictionary(input, EnglishToRussianCharacters);

        var combinations = new (string Input, IDictionary<string, double> Frequencies)[]
        {
            (input, _frequencies.EnglishBigramFrequencies),
            (input, _frequencies.RussianBigramFrequencies),

            (englishTranslation, _frequencies.EnglishBigramFrequencies),
            (englishTranslation, _frequencies.RussianBigramFrequencies),

            (russianTranslation, _frequencies.EnglishBigramFrequencies),
            (russianTranslation, _frequencies.RussianBigramFrequencies)
        };

        var scores = combinations.Select(x => new
        {
            Input = x.Input,
            Naturality = MeasureNaturalness(x.Input, x.Frequencies)
        });

        var bestResult = scores.MaxBy(x => x.Naturality);

        return bestResult.Naturality >= NaturalnessThreshold
            ? bestResult.Input
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

    private static IDictionary<T, T> MapItems<T>(IEnumerable<T> a, IEnumerable<T> b) =>
        a.Zip(b).ToDictionary(x => x.First, x => x.Second);
} 