using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class DirectMessage
{
    [Key]
    public int Id { get; set; }

    // Foreign Keys
    public int SenderId { get; set; }
    public User Sender { get; set; }

    public int ReceiverId { get; set; }
    public User Receiver { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; } = false;
}