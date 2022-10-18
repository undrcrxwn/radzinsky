namespace Radzinsky.Application.Models.AuthorizationResults;

/// <summary>
/// No permission can be provided for acting against superadmin
/// </summary>
public record FailedAgainstSuperadmin : Failure;