namespace Radzinsky.Application.Utils;

public static class EnumerableExtensions
{
    public static T PickRandom<T>(this IEnumerable<T> elements)
    {
        return elements.ElementAt(Random.Shared.Next(elements.Count()));
    }
}