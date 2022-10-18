using Radzinsky.Application.Models.Checkpoints;

namespace Radzinsky.Application.Abstractions;

public interface ICheckpointMemoryService
{
    public void SetCheckpoint(long userId, Checkpoint checkpoint, TimeSpan? duration = null);
    public Checkpoint? GetLocalCheckpoint(long userId, long chatId, string handlerTypeName);
    public void ResetCheckpoint(long userId);
    public Checkpoint? GetCheckpoint(long userId);
}