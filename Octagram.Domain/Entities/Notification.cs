using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class Notification
{
    [Key]
    public int Id { get; set; }

    // Foreign Key
    public int RecipientId { get; set; }
    public User? Recipient { get; set; }

    public int? SenderId { get; set; } // Can be null for system notifications
    public User? Sender { get; set; }

    [Required]
    public string Type { get; set; } // e.g., "like", "comment", "follow", "message"

    public int? TargetId { get; set; } // PostId, CommentId, etc. (depends on Type)

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; } = false;
}