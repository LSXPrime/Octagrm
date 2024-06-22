namespace Octagram.Application.DTOs;

public class FollowDto
{
    public int Id { get; set; }
    public int FollowerId { get; set; }
    public int FollowingId { get; set; }
}