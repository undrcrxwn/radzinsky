using Radzinsky.Application.Models;

namespace Radzinsky.Application.Abstractions;

public interface IInteractionService
{
    public MentionCheckpoint IssueMentionCheckpoint(long userId);
    public CommandCheckpoint IssueCheckpoint(string name, string handlerTypeName, long userId);
    public Checkpoint? TryGetCurrentCheckpoint(long userId);
    public void ResetCheckpoint(long userId);
    public Task SetPreviousReplyMessageIdAsync(string handlerTypeName, long chatId, int messageId);
    public Task<int?> TryGetPreviousReplyMessageIdAsync(string handlerTypeName, long chatId);
}