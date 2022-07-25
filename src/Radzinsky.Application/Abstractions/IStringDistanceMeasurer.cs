namespace Radzinsky.Application.Abstractions;

public interface IStringDistanceMeasurer
{
    public int MeasureDistance(ReadOnlySpan<char> a, ReadOnlySpan<char> b);
}