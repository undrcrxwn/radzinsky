using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Domain.Models;

public record ImperativeCall(ImperativeType ImperativeType, IEnumerable<object> Arguments);