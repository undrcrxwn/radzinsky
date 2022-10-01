using System.ComponentModel.DataAnnotations;

namespace Radzinsky.Domain.Models.Entities;

public class User
{
    public long Id { get; set; }
    
    [MaxLength(50)]
    public string FirstName { get; set; }
    
    [MaxLength(200)]
    public string? Bio { get; set; }
}