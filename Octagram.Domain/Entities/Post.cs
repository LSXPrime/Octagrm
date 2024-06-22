using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class Post
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ImageUrl { get; set; }
    public string? Caption { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Key
    public int UserId { get; set; }
    public User User { get; set; }

    // Navigation Properties
    public List<Like> Likes { get; set; }
    public List<Comment> Comments { get; set; }
    public List<PostHashtag> PostHashtags { get; set; } 
}