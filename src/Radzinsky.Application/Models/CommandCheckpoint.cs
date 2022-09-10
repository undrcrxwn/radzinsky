namespace Radzinsky.Application.Models;

public record CommandCheckpoint(string Name, TimeSpan Duration, string CommandTypeName)
    : Checkpoint(Name, Duration);