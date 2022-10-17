using System.ComponentModel.DataAnnotations;

namespace Radzinsky.Domain.Models.Entities;

public class ChatPortal
{
    [Key] public long ChatId { get; set; }
    public Chat Chat { get; set; }
    public int MemberCount { get; set; }
}