using System.Diagnostics;
using Radzinsky.Application.Abstractions;

namespace Radzinsky.Application.Services;

public class RuntimeInfoService : IRuntimeInfoService
{
    public TimeSpan GetUptime()
    {
        using var process = Process.GetCurrentProcess();
        return DateTime.UtcNow - process.StartTime.ToUniversalTime();
    }
}