using Radzinsky.Application.Abstractions;
using Radzinsky.Domain.Models.Entities;
using OneOf;

namespace Radzinsky.Application.Permissions;

public static class BanPermission
{
    
    public OneOf<AccessGranted, AccessDenied> Authorize(ChatMember member) =>
        member.IsChatAdministrator ||
        member.Role is not null &&
        member.Role.Permissions.Contains(nameof(BanPermission))
        ? ;
}