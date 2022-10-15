using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Radzinsky.Application.Abstractions;

namespace Radzinsky.Application.Services;

public class Md5HashingService : IHashingService
{
    private readonly int _keyLength;

    public Md5HashingService(IConfiguration configuration) =>
        _keyLength = configuration.GetValue<int>("Callbacks:CallbackHandlerKeyLength");
    
    public string HashKey(string key)
    {
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(key));
        return string.Join(string.Empty, hashBytes
            .Select(x => x.ToString("x2"))
            .Take(_keyLength));
    }
}