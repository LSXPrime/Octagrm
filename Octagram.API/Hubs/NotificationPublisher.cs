using Microsoft.AspNetCore.SignalR;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;

namespace Octagram.API.Hubs;

public class NotificationPublisher(IHubContext<NotificationHub> notificationHub) : INotificationPublisher
{
    /// <summary>
    /// Publishes a notification to the specified recipient via SignalR.
    /// </summary>
    /// <param name="notification">The notification to publish.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task PublishNotificationAsync(NotificationDto notification)
    {
        await notificationHub.Clients.Group(notification.RecipientId.ToString())
            .SendAsync("ReceiveNotification", notification);
    }
}