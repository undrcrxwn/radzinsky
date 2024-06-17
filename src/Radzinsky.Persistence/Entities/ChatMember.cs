namespace Radzinsky.Persistence.Entities;

public class ChatMember
{
    public long Id { get; set; }

    public User User { get; set; }
    public long UserId { get; set; }

    public Role? Role { get; set; }
    public int? RoleId { get; set; }
}