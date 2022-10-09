using System.Globalization;
using Newtonsoft.Json.Linq;
using Radzinsky.Application.Extensions;

namespace Radzinsky.Application.Models.Resources;

public abstract class ResourcesBase
{
    private readonly JObject _data;

    public ResourcesBase(JObject data) =>
        _data = data;

    public string GetRandom(string key, params object[] args) =>
        string.Format(CultureInfo.InvariantCulture, GetRandom<string>(key), args);
    
    public T GetRandom<T>(string key) =>
        GetMany<T>(key).PickRandom();
    
    public IEnumerable<string> GetMany(string key, params object[] args) =>
        GetMany<string>(key)
            .Select(x => string.Format(CultureInfo.InvariantCulture, x, args));
    
    public IEnumerable<T> GetMany<T>(string key) =>
        _data.GetValue(key)!.Values<T>()!;
    
    public IEnumerable<T?> GetManyOrEmpty<T>(string key) =>
        _data.GetValue(key)?.Values<T>() ?? Enumerable.Empty<T>();
    
    public string Get(string key, params object[] args) =>
        string.Format(CultureInfo.InvariantCulture, Get<string>(key), args);
    
    public T Get<T>(string key) =>
        _data.GetValue(key)!.Value<T>()!;
}