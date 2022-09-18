namespace Radzinsky.Application.Abstractions;

public interface INewsService
{
    public Task<IEnumerable<string>> GetTitlesAsync(DateTime date);
}