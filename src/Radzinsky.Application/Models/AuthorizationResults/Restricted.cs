namespace Radzinsky.Application.Models.AuthorizationResults;

/// <summary>
/// Access is intentionally blocked as an exception for default authorization rules
/// </summary>
public record Restricted : Failure;