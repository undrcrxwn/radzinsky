using System.Text.RegularExpressions;

namespace Radzinsky.Framework.Routing.StringDistance;

public static partial class StringExtensions
{
    private const string RussianQwertyLayout = @"ёйцукенгшщзхъфывапролджэячсмитьбюЁЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ.!""№;%:?";
    private const string EnglishQwertyLayout = @"`qwertyuiop[]asdfghjkl;'zxcvbnm,.~QWERTYUIOP{}ASDFGHJKL:""ZXCVBNM<>/!@#$%^&";
    private static readonly Dictionary<char, char> RussianQwertyByEnglishQwerty;

    static StringExtensions() =>
        RussianQwertyByEnglishQwerty = EnglishQwertyLayout.Zip(RussianQwertyLayout)
            .ToDictionary(x => x.First, x => x.Second);

    public static string NormalizeForStringDistanceCalculation(this string text)
    {
        var russianQwertyTranslatedCharacters = text.Select(character =>
        {
            var presentInDictionary = RussianQwertyByEnglishQwerty.TryGetValue(character, out var translatedCharacter);
            return presentInDictionary ? translatedCharacter : character;
        });

        return new string(russianQwertyTranslatedCharacters.ToArray())
            .ToLower()
            .RemovePunctuation()
            .Replace('а', 'о')
            .Replace('ъ', 'ь')
            .Replace('е', 'и')
            .Replace('ё', 'е')
            .Replace('э', 'е')
            .RemoveAdjacentDuplicates()
            .Trim();
    }

    private static string RemovePunctuation(this string text) =>
        GetPunctuationPattern().Replace(text, "$1");

    [GeneratedRegex(@"[.,!?\\-]")]
    private static partial Regex GetPunctuationPattern();

    private static string RemoveAdjacentDuplicates(this string text) =>
        GetAdjacentDuplicatesPattern().Replace(text, "$1");

    [GeneratedRegex(@"(.)\1+")]
    private static partial Regex GetAdjacentDuplicatesPattern();
}