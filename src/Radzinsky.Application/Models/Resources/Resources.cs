using System.Globalization;
using Newtonsoft.Json.Linq;
using Radzinsky.Application.Extensions;

namespace Radzinsky.Application.Models.Resources;

public class Resources
{
    private JObject Data;

    public Resources(JObject data) =>
        Data = data;

    public string GetRandom(string key, params object[] args) =>
        string.Format(CultureInfo.InvariantCulture, GetRandom<string>(key), args);
    
    public T GetRandom<T>(string key) =>
        GetMany<T>(key).PickRandom();
    
    public IEnumerable<string> GetMany(string key, params object[] args) =>
        GetMany<string>(key)
            .Select(x => string.Format(CultureInfo.InvariantCulture, x, args));
    
    public IEnumerable<T> GetMany<T>(string key) =>
        Data.GetValue(key).Values<T>();
    
    public IEnumerable<T> GetManyOrEmpty<T>(string key) =>
        Data.GetValue(key)?.Values<T>() ?? Enumerable.Empty<T>();
    
    public string Get(string key, params object[] args) =>
        string.Format(CultureInfo.InvariantCulture, Get<string>(key), args);
    
    public T Get<T>(string key) =>
        Data.GetValue(key).Value<T>();
}