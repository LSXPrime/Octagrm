namespace Octagram.Application.DTOs;

public class CreateDirectMessageRequest
{
    public int ReceiverId { get; set; }
    public string Content { get; set; } 
}