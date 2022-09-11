using Microsoft.Extensions.Caching.Memory;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Services;

public class InteractionService : IInteractionService
{
    private readonly IMemoryCache _cache;

    public InteractionService(IMemoryCache cache) =>
        _cache = cache;

    public MentionCheckpoint IssueMentionCheckpoint(string name, long userId)
    {
        var checkpoint = new MentionCheckpoint(name, TimeSpan.FromSeconds(15));
        SetUserCheckpoint(userId, checkpoint);
        return checkpoint;
    }

    public CommandCheckpoint IssueCommandCheckpoint(string name, string commandTypeName, long userId)
    {
        var checkpoint = new CommandCheckpoint(name, TimeSpan.FromMinutes(1), commandTypeName);
        SetUserCheckpoint(userId, checkpoint);
        return checkpoint;
    }

    public Checkpoint? TryGetCurrentCheckpoint(long userId)
    {
        _cache.TryGetValue(GetCheckpointCacheEntryName(userId), out Checkpoint? checkpoint);
        return checkpoint;
    }

    public void ResetCheckpoint(long userId) =>
        _cache.Remove(GetCheckpointCacheEntryName(userId));

    public async Task SetPreviousReplyMessageIdAsync(string commandTypeName, long chatId, int messageId) =>
        _cache.Set(GetReplyMessageIdCacheEntryName(commandTypeName, chatId), messageId);

    public async Task<int?> TryGetPreviousReplyMessageIdAsync(string commandTypeName, long chatId)
    {
        _cache.TryGetValue(GetReplyMessageIdCacheEntryName(commandTypeName, chatId), out int? messageId);
        return messageId;
    }

    private void SetUserCheckpoint(long userId, Checkpoint checkpoint) =>
        _cache.Set(GetCheckpointCacheEntryName(userId), checkpoint, checkpoint.Duration);
    
    private static string GetCheckpointCacheEntryName(long userId) => $"Checkpoint-{userId}";
    
    private static string GetReplyMessageIdCacheEntryName(string commandTypeName, long chatId) =>
        $"Reply-{commandTypeName}-{chatId}";
}