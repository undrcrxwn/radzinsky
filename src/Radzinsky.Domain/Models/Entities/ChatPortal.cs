using System.ComponentModel.DataAnnotations;

namespace Radzinsky.Domain.Models.Entities;

public class ChatPortal
{
    public Chat Chat { get; set; }
    public int MemberCount { get; set; }
}