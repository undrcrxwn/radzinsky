using Microsoft.Extensions.Caching.Memory;
using Radzinsky.Application.Abstractions;

namespace Radzinsky.Application.Services;

public class ReplyMemoryService : IReplyMemoryService
{
    private const string PreviousReplyEntryNameTemplate = "__Reply__Previous__{0}__{1}";
    private static readonly TimeSpan MemoryDuration = TimeSpan.FromHours(6);
    private readonly IMemoryCache _cache;

    public ReplyMemoryService(IMemoryCache cache) =>
        _cache = cache;
    
    public void SetPreviousReplyMessageId(string handlerTypeName, long chatId, int messageId) =>
        _cache.Set(GetPreviousReplyEntryName(handlerTypeName, chatId), messageId, MemoryDuration);

    public int? TryGetPreviousReplyMessageId(string handlerTypeName, long chatId) =>
        _cache.TryGetValue(GetPreviousReplyEntryName(handlerTypeName, chatId), out int messageId)
            ? messageId
            : null;

    private static string GetPreviousReplyEntryName(string handlerTypeName, long chatId) =>
        string.Format(PreviousReplyEntryNameTemplate, handlerTypeName, chatId);
}