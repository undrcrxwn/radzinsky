namespace Radzinsky.Application.Extensions;

public static class TimeSpanExtensions
{
    public static string ToShortString(this TimeSpan span) =>
        string.Join(' ', string.Format("{0} {1} {2} {3}",
            span.Days > 0
                ? $"{span.Days.ToString()}д"
                : string.Empty,
            span.Hours > 0
                ? $"{span.Hours.ToString()}ч"
                : string.Empty,
            span.Minutes > 0
                ? $"{span.Minutes.ToString()}м"
                : string.Empty,
            span.Seconds > 0 || span.TotalSeconds == 0
                ? $"{span.Seconds.ToString()}с"
                : string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));
}