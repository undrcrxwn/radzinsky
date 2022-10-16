namespace Radzinsky.Application.Abstractions;

public interface IReplyMemoryService
{
    public void SetPreviousReplyMessageId(string handlerTypeName, long chatId, int messageId);
    public int? TryGetPreviousReplyMessageId(string handlerTypeName, long chatId);
}