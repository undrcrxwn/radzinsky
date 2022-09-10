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

    public Checkpoint? GetCurrentCheckpoint(long userId)
    {
        _cache.TryGetValue(GetCacheEntryName(userId), out Checkpoint? checkpoint);
        return checkpoint;
    }

    public void ResetCheckpoint(long userId) =>
        _cache.Remove(GetCacheEntryName(userId));

    private void SetUserCheckpoint(long userId, Checkpoint checkpoint) =>
        _cache.Set(GetCacheEntryName(userId), checkpoint, checkpoint.Duration);
    
    private static string GetCacheEntryName(long userId) => $"Checkpoint-{userId}";
}