using Radzinsky.Application.Models.AuthorizationResults;
using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Application.Abstractions;

public interface IAuthorizationService
{
    public Task<AuthorizationResult> AuthorizeAsync(long userId, long chatId, ChatMemberPermissions permissions);
    public Task<AuthorizationResult> AuthorizeAgainstAsync(long userId, long targetId, long chatId, ChatMemberPermissions permissions);
}