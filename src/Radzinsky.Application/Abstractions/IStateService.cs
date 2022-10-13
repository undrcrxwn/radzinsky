namespace Radzinsky.Application.Abstractions;

public interface IStateService
{
    public T? ReadState<T>(string stateKey);
    public void WriteState(string stateKey, object state);
}