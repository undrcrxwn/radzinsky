using Radzinsky.Application.Models;

namespace Radzinsky.Application.Abstractions;

public interface IInteractionService
{
    public Checkpoint IssueCheckpoint(string name, string commandTypeName, long userId);
    public Checkpoint? GetCurrentCheckpoint(long userId);
}