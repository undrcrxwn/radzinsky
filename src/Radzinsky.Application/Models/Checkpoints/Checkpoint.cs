namespace Radzinsky.Application.Models.Checkpoints;

public abstract record Checkpoint(string Name, TimeSpan Duration);