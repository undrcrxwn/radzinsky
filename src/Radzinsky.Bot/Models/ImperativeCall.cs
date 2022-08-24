using Radzinsky.Bot.Enumerations;

namespace Radzinsky.Bot.Models;

public record ImperativeCall(ImperativeType ImperativeType, IEnumerable<object> Arguments);