namespace Radzinsky.Domain.Models.Entities;

public class ChatMember
{
    public long ChatId { get; set; }
    public Chat Chat { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
    public bool IsChatAdministrator { get; set; }
    public Role? Role { get; set; }
}