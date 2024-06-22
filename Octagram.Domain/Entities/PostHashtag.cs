using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class PostHashtag
{
    [Key]
    public int Id { get; set; }

    // Foreign Keys
    public int PostId { get; set; }
    public Post Post { get; set; }

    public int HashtagId { get; set; }
    public Hashtag Hashtag { get; set; }
}