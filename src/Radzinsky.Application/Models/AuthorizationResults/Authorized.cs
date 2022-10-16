namespace Radzinsky.Application.Models.AuthorizationResults;

/// <summary>
/// All authorization rules are satisfied
/// </summary>
/// <param name="AsSuperadmin">Access was granted because the user is chat's creator or Telegram admin</param>
public record Authorized(bool AsSuperadmin) : Success;