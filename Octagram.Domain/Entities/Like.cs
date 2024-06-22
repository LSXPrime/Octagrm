using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class Like
{
    [Key]
    public int Id { get; set; }

    // Foreign Keys
    public int UserId { get; set; }
    public User User { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }
}