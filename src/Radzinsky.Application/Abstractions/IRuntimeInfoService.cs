namespace Radzinsky.Application.Abstractions;

public interface IRuntimeInfoService
{
    public TimeSpan GetUptime();
}