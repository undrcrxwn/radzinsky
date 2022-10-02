namespace Radzinsky.Domain.Models.Entities;

public class Chat
{
    public long Id { get; set; }
    public IEnumerable<ChatRole> Roles { get; set; } = null!;
}