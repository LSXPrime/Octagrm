using System.ComponentModel.DataAnnotations;

namespace Octagram.Domain.Entities;

public class Follow
{
    [Key]
    public int Id { get; set; }

    // Foreign Keys
    public int FollowerId { get; set; }
    public User Follower { get; set; }

    public int FollowingId { get; set; }
    public User Following { get; set; }
}