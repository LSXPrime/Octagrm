using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public string Token { get; set; }
    
    [Required]
    public DateTime Expires { get; set; }
    
    public string Role { get; set; } = "User";
}