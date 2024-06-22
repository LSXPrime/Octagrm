using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Octagram.Application.DTOs;
using Octagram.Application.Exceptions;
using Octagram.Application.Interfaces;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;

namespace Octagram.Application.Services;

public class NotificationService(
    INotificationRepository notificationRepository,
    IPostRepository postRepository,
    ICommentRepository commentRepository,
    IUserRepository userRepository,
    INotificationPublisher notificationPublisher,
    IMapper mapper)
    : INotificationService
{
    /// <summary>
    /// Creates a new notification.
    /// </summary>
    /// <param name="notification">The notification to be created.</param>
    /// <exception cref="BadRequestException">Thrown if the sender or recipient is not found.</exception>
    public async Task CreateNotificationAsync(Notification notification)
    {
        if (notification.Sender == null || notification.Recipient == null)
        {
            throw new BadRequestException("Sender or recipient not found.");
        }

        await notificationRepository.AddAsync(notification);
        await notificationPublisher.PublishNotificationAsync(mapper.Map<NotificationDto>(notification));
    }

    /// <summary>
    /// Retrieves all notifications for the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve notifications for.</param>
    /// <returns>A collection of notification DTOs representing the user's notifications.</returns>
    public async Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(int userId)
    {
        var notifications = await notificationRepository.GetNotificationsByUserIdAsync(userId);
        return mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    /// <summary>
    /// Retrieves a paged list of notifications for the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve notifications for.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of notifications per page.</param>
    /// <returns>A paged result containing a collection of notification DTOs.</returns>
    public async Task<PagedResult<NotificationDto>> GetNotificationsByUserIdAsync(int userId, int page, int pageSize)
    {
        var query = notificationRepository.GetAllAsync();

        // Apply pagination
        var skip = (page - 1) * pageSize;
        var notifications = await query
            .Where(n => n.RecipientId == userId)
            .Include(n => n.Sender)
            .OrderByDescending(n => n.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<NotificationDto>
        {
            Items = mapper.Map<IEnumerable<NotificationDto>>(notifications),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = notifications.Count
        };
    }

    /// <summary>
    /// Marks all notifications for the specified user as read.
    /// </summary>
    /// <param name="userId">The ID of the user whose notifications should be marked as read.</param>
    public Task MarkAllNotificationsAsReadAsync(int userId)
    {
        var notifications = notificationRepository.GetAllAsync();
        return notifications
            .Where(n => n.RecipientId == userId && !n.IsRead)
            .ForEachAsync(n => n.IsRead = true);
    }

    /// <summary>
    /// Marks the specified notifications as read for the specified user.
    /// </summary>
    /// <param name="notificationIds">The IDs of the notifications to mark as read.</param>
    /// <param name="userId">The ID of the user whose notifications should be marked as read.</param>
    public async Task MarkNotificationsAsReadAsync(IEnumerable<int> notificationIds, int userId)
    {
        var notificationsToUpdate = await notificationRepository.FindAsync(n =>
            notificationIds.Contains(n.Id) && n.RecipientId == userId).ToListAsync();

        foreach (var notification in notificationsToUpdate)
        {
            notification.IsRead = true;
        }
    }

    /// <summary>
    /// Marks the specified notification as read for the specified user.
    /// </summary>
    /// <param name="notificationId">The ID of the notification to mark as read.</param>
    /// <param name="userId">The ID of the user whose notification should be marked as read.</param>
    public async Task MarkNotificationAsReadAsync(int notificationId, int userId)
    {
        var notificationsByUser = await notificationRepository.GetNotificationsByUserIdAsync(userId);
        var notification = notificationsByUser.FirstOrDefault(n => n.Id == notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await notificationRepository.UpdateAsync(notification);
        }
    }

    /// <summary>
    /// Creates a notification for a like on a post.
    /// </summary>
    /// <param name="postId">The ID of the post that was liked.</param>
    /// <param name="userIdLikingPost">The ID of the user who liked the post.</param>
    /// <exception cref="NotFoundException">Thrown if the post is not found.</exception>
    public async Task CreateLikeNotificationAsync(int postId, int userIdLikingPost)
    {
        var post = await postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new NotFoundException("Post not found.");
        }

        // Don't create a notification if the user likes their own post
        if (post.UserId == userIdLikingPost)
        {
            return;
        }

        var notification = new Notification
        {
            RecipientId = post.UserId,
            SenderId = userIdLikingPost,
            Sender = await userRepository.GetByIdAsync(userIdLikingPost),
            Recipient = await userRepository.GetByIdAsync(post.UserId),
            Type = "like",
            TargetId = postId,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await CreateNotificationAsync(notification);
    }

    /// <summary>
    /// Creates a notification for a comment on a post.
    /// </summary>
    /// <param name="commentId">The ID of the comment that was added.</param>
    /// <param name="userIdCommenting">The ID of the user who commented on the post.</param>
    /// <exception cref="NotFoundException">Thrown if the comment is not found.</exception>
    public async Task CreateCommentNotificationAsync(int commentId, int userIdCommenting)
    {
        var comment = await commentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            throw new NotFoundException("Comment not found.");
        }

        // Don't create a notification if the user comments on their own post
        if (comment.UserId == comment.Post.UserId)
        {
            return;
        }

        var notification = new Notification
        {
            RecipientId = comment.Post.UserId,
            SenderId = userIdCommenting,
            Sender = await userRepository.GetByIdAsync(userIdCommenting),
            Recipient = await userRepository.GetByIdAsync(comment.Post.UserId),
            Type = "comment",
            TargetId = commentId,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await CreateNotificationAsync(notification);
    }

    /// <summary>
    /// Creates a notification for a follow event.
    /// </summary>
    /// <param name="followerId">The ID of the user who followed another user.</param>
    /// <param name="followingId">The ID of the user who was followed.</param>
    public async Task CreateFollowNotificationAsync(int followerId, int followingId)
    {
        // Don't create a notification if a user tries to follow themselves
        if (followerId == followingId)
        {
            return;
        }

        var notification = new Notification
        {
            RecipientId = followingId,
            SenderId = followerId,
            Sender = await userRepository.GetByIdAsync(followerId),
            Recipient = await userRepository.GetByIdAsync(followingId),
            Type = "follow",
            TargetId = null,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await CreateNotificationAsync(notification);
    }
}