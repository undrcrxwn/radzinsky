using System.Security.Cryptography;
using System.Text;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Abstractions;

public interface ICallbackQueryHandler
{
    public Task HandleCallbackQueryAsync(CallbackQueryContext context, CancellationToken token);

    protected sealed string HashIssuerKey(string key) =>
        Encoding.UTF8.GetString(MD5.HashData(Encoding.UTF8.GetBytes(key)));
}