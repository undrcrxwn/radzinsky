namespace Radzinsky.Domain.Models.Entities;

public class Chat
{
    public long Id { get; set; }
    public IEnumerable<ChatMemberRole> Roles { get; set; } = null!;
}