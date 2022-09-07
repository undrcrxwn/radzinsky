namespace Radzinsky.Application.Abstractions;

public interface IStringDistanceMeasurer
{
    public int MeasureDistance(string a, string b);
}