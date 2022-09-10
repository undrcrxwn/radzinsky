namespace Radzinsky.Application.Models;

public class Checkpoint
{
    public string Name { get; init; }
    public string CommandTypeName { get; init; }
    public TimeSpan Duration { get; init; }
}