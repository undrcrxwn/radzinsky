using Microsoft.Extensions.Caching.Memory;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Services;

public class InteractionService : IInteractionService
{
    private readonly IMemoryCache _cache;
    
    public InteractionService(IMemoryCache cache) =>
        _cache = cache;
    
    public Checkpoint IssueCheckpoint(string name, string commandTypeName, long userId)
    {
        var checkpoint = new Checkpoint()
        {
            CommandTypeName = commandTypeName,
            Name = name,
            Duration = TimeSpan.FromMinutes(5)
        };
        
        _cache.Set(GetCacheEntryName(userId), checkpoint, checkpoint.Duration);
        return checkpoint;
    }

    public Checkpoint? GetCurrentCheckpoint(long userId) =>
        _cache.Get<Checkpoint>(GetCacheEntryName(userId));

    private static string GetCacheEntryName(long userId) => $"Checkpoint-{userId}";
}