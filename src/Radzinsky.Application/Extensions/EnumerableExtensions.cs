namespace Radzinsky.Application.Extensions;

public static class EnumerableExtensions
{
    public static T PickRandom<T>(this IEnumerable<T> items) =>
        items.ElementAt(Random.Shared.Next(items.Count()));
}