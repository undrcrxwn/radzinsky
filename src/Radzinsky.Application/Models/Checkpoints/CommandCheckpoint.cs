namespace Radzinsky.Application.Models.Checkpoints;

public record CommandCheckpoint(string Name, TimeSpan Duration, string CommandTypeName)
    : Checkpoint(Name, Duration);