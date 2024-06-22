using Microsoft.AspNetCore.Mvc;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;
using Octagram.API.Attributes;
using System.Security.Claims;

namespace Octagram.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    /// <summary>
    /// Gets the notifications for the authenticated user.
    /// </summary>
    /// <param name="page">The page number for pagination (default: 1).</param>
    /// <param name="pageSize">The number of notifications per page (default: 10).</param>
    /// <returns>
    /// Returns an OK response with a list of NotificationDto objects representing the user's notifications.
    /// </returns>
    [HttpGet]
    [AuthorizeMiddleware("User")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10 
    )
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var notifications = await notificationService.GetNotificationsByUserIdAsync(userId, page, pageSize);
        return Ok(notifications);
    }

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    /// <param name="notificationId">The ID of the notification to mark as read.</param>
    /// <returns>
    /// Returns an OK response with a message "Notification marked as read."
    /// </returns>
    [HttpPatch("{notificationId}/read")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await notificationService.MarkNotificationAsReadAsync(notificationId, userId);
        return Ok("Notification marked as read.");
    }
    
    /// <summary>
    /// Marks all notifications for the authenticated user as read.
    /// </summary>
    /// <returns>
    /// Returns an OK response with a message "Notification marked as read."
    /// </returns>
    [HttpPatch("read")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> MarkAllNotificationsAsRead()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await notificationService.MarkAllNotificationsAsReadAsync(userId);
        return Ok("Notification marked as read.");
    }
}