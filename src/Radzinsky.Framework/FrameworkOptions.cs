using System.Reflection;

namespace Radzinsky.Framework;

public class FrameworkOptions
{
    public readonly List<Assembly> AssembliesToScan = [];
    
    public class Builder
    {
        private FrameworkOptions _options = new();

        public Builder ScanAssembly(Assembly assembly)
        {
            _options.AssembliesToScan.Add(assembly);
            return this;
        }
    
        internal FrameworkOptions Build()
        {
            var options = _options;
            _options = new FrameworkOptions();
            return options;
        }
    }
}