using Radzinsky.Application.Models;

namespace Radzinsky.Application.Abstractions;

public interface IInteractionService
{
    public MentionCheckpoint IssueMentionCheckpoint(string name, long userId);
    public CommandCheckpoint IssueCommandCheckpoint(string name, string commandTypeName, long userId);
    public Checkpoint? TryGetCurrentCheckpoint(long userId);
    public void ResetCheckpoint(long userId);
    public Task SetPreviousReplyMessageIdAsync(string commandTypeName, long chatId, int messageId);
    public Task<int?> TryGetPreviousReplyMessageIdAsync(string commandTypeName, long chatId);
}