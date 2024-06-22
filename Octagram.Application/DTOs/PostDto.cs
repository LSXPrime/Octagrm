namespace Octagram.Application.DTOs;

public class PostDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public string Caption { get; set; }
    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }
    public UserDto User { get; set; }

    public int LikesCount { get; set; }
    public bool IsLikedByUser { get; set; }

    public List<CommentDto> Comments { get; set; } 
}