using Microsoft.Extensions.Primitives;

namespace Radzinsky.Framework.Routing.StringDistance;

public class StringView
{
    public readonly string Text;
    public readonly StringSegment[] TextWords;
    public readonly string NormalizedText;
    public readonly StringSegment[] NormalizedTextWords;

    public StringView(string text)
    {
        if (text.StartsWith(' ') || text.EndsWith(' '))
            throw new ArgumentException("Text must be trimmed.", nameof(text));

        Text = text;
        TextWords = new StringSegment(text).Split([' ']).Where(segment => segment.Length > 0).ToArray();
        NormalizedText = text.NormalizeForStringDistanceCalculation();
        NormalizedTextWords = new StringSegment(NormalizedText).Split([' ']).ToArray();
    }

    public StringSegment SelectTextWordsFrom(int from) => SelectTextWords(from, TextWords.Length - from);

    public StringSegment SelectTextWords(int from, int count)
    {
        if (from >= TextWords.Length)
            return new StringSegment(Text, Text.Length - 1, 0);

        var to = Math.Min(from + count - 1, TextWords.Length - 1);
        var segmentLength = TextWords[to].Offset - TextWords[from].Offset + TextWords[to].Length;
        return new StringSegment(Text, TextWords[from].Offset, segmentLength);
    }

    public StringSegment SelectNormalizedTextWords(int from, int count)
    {
        if (from >= NormalizedTextWords.Length)
            return new StringSegment(Text, Text.Length - 1, 0);
        
        var to = Math.Min(from + count - 1, NormalizedTextWords.Length - 1);
        var segmentLength = NormalizedTextWords[to].Offset - NormalizedTextWords[from].Offset + NormalizedTextWords[to].Length;
        return new StringSegment(NormalizedText, NormalizedTextWords[from].Offset, segmentLength);
    }
}