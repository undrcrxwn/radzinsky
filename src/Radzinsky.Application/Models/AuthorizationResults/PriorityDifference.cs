namespace Radzinsky.Application.Models.AuthorizationResults;

/// <summary>
/// Target has a much higher priority
/// </summary>
/// <param name="ActualPriority">Priority of the member who asks for permission</param>
/// <param name="TargetPriority">Priority of the target member</param>
public record PriorityDifference(int ActualPriority, int TargetPriority) : Failure;