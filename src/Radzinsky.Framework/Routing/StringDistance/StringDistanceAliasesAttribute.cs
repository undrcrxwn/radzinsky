namespace Radzinsky.Framework.Routing.StringDistance;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class StringDistanceAliasesAttribute(params string[] aliases) : Attribute
{
    public readonly IReadOnlyCollection<(string Value, int WordCount)> Aliases = aliases
        .Select(alias => alias.NormalizeForStringDistanceCalculation())
        .OrderByDescending(alias => alias.Length)
        .Select(alias => (alias, alias.Count(character => character == ' ') + 1))
        .ToArray();
}