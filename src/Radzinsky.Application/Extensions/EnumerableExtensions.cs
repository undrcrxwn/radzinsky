using System.Diagnostics.CodeAnalysis;

namespace Radzinsky.Application.Extensions;

public static class EnumerableExtensions
{
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public static T PickRandom<T>(this IEnumerable<T> items) =>
        items.ElementAt(Random.Shared.Next(items.Count()));
}