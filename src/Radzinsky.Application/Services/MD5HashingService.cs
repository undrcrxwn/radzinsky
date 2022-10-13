using System.Security.Cryptography;
using System.Text;
using Radzinsky.Application.Abstractions;

namespace Radzinsky.Application.Services;

public class MD5HashingService : IHashingService
{
    private const int HashBytesCount = 5;
    
    public string HashKey(string key)
    {
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(key));
        return string.Join(string.Empty, hashBytes
            .Take(HashBytesCount)
            .Select(x => x.ToString("x2")));
    }
}