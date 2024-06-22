using Octagram.Application.DTOs;

namespace Octagram.Application.Interfaces;

public interface INotificationPublisher
{
    /// <summary>
    /// Publishes a notification to the specified recipient via real-time messaging service.
    /// </summary>
    /// <param name="notification">The notification to publish.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task PublishNotificationAsync(NotificationDto notification);
}