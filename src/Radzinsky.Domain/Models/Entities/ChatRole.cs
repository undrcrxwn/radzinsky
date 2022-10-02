using System.ComponentModel.DataAnnotations;
using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Domain.Models.Entities;

public class ChatRole
{
    public Guid Id { get; set; }
    
    [MaxLength(50)]
    public string Title { get; set; }

    public int Priority { get; set; }
    
    public IEnumerable<MemberPermissions> Permissions { get; set; }
}