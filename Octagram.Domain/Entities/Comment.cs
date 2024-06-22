using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public int UserId { get; set; }
    public User User { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }
}