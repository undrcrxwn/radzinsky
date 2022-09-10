using System.ComponentModel.DataAnnotations;

namespace Radzinsky.Domain.Models;

public class UserBio
{
    [Key]
    public long UserId { get; set; }
    public string Description { get; set; }
}