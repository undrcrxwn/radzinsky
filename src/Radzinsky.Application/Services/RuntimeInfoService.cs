using System.Diagnostics;
using Radzinsky.Application.Abstractions;

namespace Radzinsky.Application.Services;

public class RuntimeInfoService : IRuntimeInfoService
{
    public TimeSpan GetUptime() =>
        DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
}