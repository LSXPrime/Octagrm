using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }

    // Navigation Properties
    public List<Post> Posts { get; set; }
    public List<Like> Likes { get; set; }
    public List<Comment> Comments { get; set; }
    public List<Story> Stories { get; set; } 
    public List<Follow> Following { get; set; } = [];
    public List<Follow> Followers { get; set; } = [];
    public List<DirectMessage> SentMessages { get; set; }
    public List<DirectMessage> ReceivedMessages { get; set; }
    public List<Notification> Notifications { get; set; }
}