namespace Radzinsky.Persistence.Entities;

public class Chat
{
    public long Id { get; set; }
    
    public List<Role> Roles { get; set; }
    public List<ChatMember> KnownMembers { get; set; }
}