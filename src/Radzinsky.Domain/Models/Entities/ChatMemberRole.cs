using System.ComponentModel.DataAnnotations;
using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Domain.Models.Entities;

public class ChatMemberRole
{
    [MaxLength(50)]
    public string Title { get; set; }

    public int Priority { get; set; }
    
    public ICollection<MemberPermissions> Permissions { get; set; }
}