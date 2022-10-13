namespace Radzinsky.Application.Abstractions;

public interface IHashingService
{
    public string HashKey(string key);
}