namespace Radzinsky.Application.Abstractions;

public interface IHolidaysService
{
    public Task<IEnumerable<string>> GetHolidaysAsync();
}