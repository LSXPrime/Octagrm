using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class Hashtag
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } 

    // Navigation Property
    public List<PostHashtag> PostHashtags { get; set; } 
}