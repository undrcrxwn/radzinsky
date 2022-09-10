namespace Radzinsky.Application.Models;

public record MentionCheckpoint(string Name, TimeSpan Duration)
    : Checkpoint(Name, Duration);