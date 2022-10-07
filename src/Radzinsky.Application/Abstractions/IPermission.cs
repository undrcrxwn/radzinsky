using Radzinsky.Domain.Models.Entities;

namespace Radzinsky.Application.Abstractions;

public interface IPermission
{
    public bool Authorize(ChatMember member);
}