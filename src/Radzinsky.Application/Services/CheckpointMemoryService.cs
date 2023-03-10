using Microsoft.Extensions.Caching.Memory;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Checkpoints;

namespace Radzinsky.Application.Services;

public class CheckpointMemoryService : ICheckpointMemoryService
{
    private const string CheckpointEntryNameTemplate = "__Checkpoint__{0}";
    private static readonly TimeSpan DefaultCheckpointDuration = TimeSpan.FromMinutes(30);
    private readonly IMemoryCache _cache;

    public CheckpointMemoryService(IMemoryCache cache) =>
        _cache = cache;
    
    public void SetCheckpoint(long userId, Checkpoint checkpoint, TimeSpan? duration = null) =>
        _cache.Set(GetCheckpointEntryName(userId), checkpoint, duration ?? DefaultCheckpointDuration);
    
    public void ResetCheckpoint(long userId) =>
        _cache.Remove(GetCheckpointEntryName(userId));

    public Checkpoint? GetLocalCheckpoint(long userId, long chatId, string handlerTypeName)
    {
        var checkpoint = GetCheckpoint(userId);
        
        if (checkpoint is null || checkpoint.HandlerTypeName != handlerTypeName)
            return null;

        return checkpoint.ChatId is null || checkpoint.ChatId == chatId
            ? checkpoint
            : null;
    }
    
    public Checkpoint? GetCheckpoint(long userId) =>
        _cache.TryGetValue(GetCheckpointEntryName(userId), out Checkpoint? checkpoint)
            ? checkpoint
            : null;
    
    private static string GetCheckpointEntryName(long userId) =>
        string.Format(CheckpointEntryNameTemplate, userId);
}