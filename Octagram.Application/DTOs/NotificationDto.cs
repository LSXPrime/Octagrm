namespace Octagram.Application.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public int RecipientId { get; set; }
    public int? SenderId { get; set; }
    public UserDto Sender { get; set; }
    public string Type { get; set; } 
    public int? TargetId { get; set; } 
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}