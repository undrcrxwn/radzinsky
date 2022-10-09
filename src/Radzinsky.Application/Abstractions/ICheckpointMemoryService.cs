using Radzinsky.Application.Models;

namespace Radzinsky.Application.Abstractions;

public interface ICheckpointMemoryService
{
    public void SetCheckpoint(long userId, Checkpoint checkpoint, TimeSpan? duration = null);
    public void ResetCheckpoint(long userId);
    public Checkpoint? TryGetCurrentCheckpoint(long userId, string handlerTypeName);
}