using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class Story
{
    [Key]
    public int Id { get; set; }

    // Foreign Key
    public int UserId { get; set; }
    public User User { get; set; }

    [Required]
    public string MediaUrl { get; set; }

    [Required]
    public string MediaType { get; set; } // e.g., "image", "video"

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; } // Optional: For time-limited stories
}