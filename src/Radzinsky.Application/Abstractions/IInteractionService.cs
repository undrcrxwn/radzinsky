using Radzinsky.Application.Models;

namespace Radzinsky.Application.Abstractions;

public interface IInteractionService
{
    public MentionCheckpoint IssueMentionCheckpoint(string name, long userId);
    public CommandCheckpoint IssueCommandCheckpoint(string name, string commandTypeName, long userId);
    public Checkpoint? GetCurrentCheckpoint(long userId);
    public void ResetCheckpoint(long userId);
}