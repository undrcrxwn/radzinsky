using System.Text.RegularExpressions;

namespace Radzinsky.Framework.Routing.RegEx;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegExPatternsAttribute(params string[] patterns) : Attribute
{
    public readonly IReadOnlyCollection<Regex> Patterns = patterns
        .Select(pattern => new Regex(pattern, RegexOptions.Compiled))
        .ToArray();
}