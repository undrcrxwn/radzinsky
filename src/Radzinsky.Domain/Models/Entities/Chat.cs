namespace Radzinsky.Domain.Models.Entities;

public class Chat
{
    public long Id { get; set; }
    public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();
    public Role? DefaultRole { get; set; }
    
    private Chat() { }

    public Chat(long id) => Id = id;
}