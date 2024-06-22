using Octagram.Application.DTOs;
using Octagram.Domain.Entities;

namespace Octagram.Application.Interfaces;

public interface INotificationService
{
    /// <summary>
    /// Retrieves all notifications for the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve notifications for.</param>
    /// <returns>A collection of notification DTOs representing the user's notifications.</returns>
    Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(int userId);

    /// <summary>
    /// Retrieves a paged list of notifications for the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve notifications for.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of notifications per page.</param>
    /// <returns>A paged result containing a collection of notification DTOs.</returns>
    Task<PagedResult<NotificationDto>> GetNotificationsByUserIdAsync(int userId, int page, int pageSize);

    /// <summary>
    /// Marks all notifications for the specified user as read.
    /// </summary>
    /// <param name="userId">The ID of the user whose notifications should be marked as read.</param>
    Task MarkAllNotificationsAsReadAsync(int userId);

    /// <summary>
    /// Marks the specified notifications as read for the specified user.
    /// </summary>
    /// <param name="notificationIds">The IDs of the notifications to mark as read.</param>
    /// <param name="userId">The ID of the user whose notifications should be marked as read.</param>
    Task MarkNotificationsAsReadAsync(IEnumerable<int> notificationIds, int userId);

    /// <summary>
    /// Marks the specified notification as read for the specified user.
    /// </summary>
    /// <param name="notificationId">The ID of the notification to mark as read.</param>
    /// <param name="userId">The ID of the user whose notification should be marked as read.</param>
    Task MarkNotificationAsReadAsync(int notificationId, int userId);

    /// <summary>
    /// Creates a new notification.
    /// </summary>
    /// <param name="notification">The notification to be created.</param>
    Task CreateNotificationAsync(Notification notification);

    /// <summary>
    /// Creates a notification for a like on a post.
    /// </summary>
    /// <param name="postId">The ID of the post that was liked.</param>
    /// <param name="userIdLikingPost">The ID of the user who liked the post.</param>
    Task CreateLikeNotificationAsync(int postId, int userIdLikingPost);

    /// <summary>
    /// Creates a notification for a comment on a post.
    /// </summary>
    /// <param name="postId">The ID of the post that was commented on.</param>
    /// <param name="userIdCommentingPost">The ID of the user who commented on the post.</param>
    Task CreateCommentNotificationAsync(int postId, int userIdCommentingPost);

    /// <summary>
    /// Creates a notification for a follow event.
    /// </summary>
    /// <param name="userIdFollower">The ID of the user who followed another user.</param>
    /// <param name="userIdFollowing">The ID of the user who was followed.</param>
    Task CreateFollowNotificationAsync(int userIdFollower, int userIdFollowing);
}