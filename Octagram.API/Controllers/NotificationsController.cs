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
        var userId = GetCurrentUserId();
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
    [HttpPatch("{notificationId:int}/read")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
    {
        var userId = GetCurrentUserId();
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
        var userId = GetCurrentUserId();
        await notificationService.MarkAllNotificationsAsReadAsync(userId);
        return Ok("Notification marked as read.");
    }
    
    /// <summary>
    /// Gets the current user's ID.
    /// </summary>
    /// <returns>
    /// Returns the current user's ID.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user ID is not found in the claims.</exception>
    [NonAction]
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in claims."); 
        }
        return userId;
    }
}