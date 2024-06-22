namespace Octagram.Application.DTOs;

public class DirectMessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public UserDto Sender { get; set; }
    public int ReceiverId { get; set; }
    public UserDto Receiver { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}