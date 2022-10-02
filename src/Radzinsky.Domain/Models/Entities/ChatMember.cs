namespace Radzinsky.Domain.Models.Entities;

public class ChatMember
{
    public Chat Chat { get; set; }
    public User User { get; set; }
    public bool IsChatAdministrator { get; set; }
    public ChatRole? Role { get; set; }
}