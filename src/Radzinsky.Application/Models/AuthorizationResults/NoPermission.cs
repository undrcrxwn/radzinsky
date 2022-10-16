namespace Radzinsky.Application.Models.AuthorizationResults;

/// <summary>
/// Member has no permission that could satisfy the default authorization rules
/// </summary>
public record NoPermission() : Failure;