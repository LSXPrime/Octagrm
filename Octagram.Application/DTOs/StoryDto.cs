namespace Octagram.Application.DTOs;

public class StoryDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string MediaUrl { get; set; }
    public string MediaType { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}