namespace Radzinsky.Persistence.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Priority { get; set; }

    public Chat Chat { get; set; }
    public long ChatId { get; set; }

    public List<ChatMember> ChatMembers { get; set; }
}