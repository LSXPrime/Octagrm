using Microsoft.AspNetCore.SignalR;
using Octagram.Application.DTOs; 
using Octagram.Application.Interfaces;

namespace Octagram.API.Hubs
{
    public class NotificationHub(INotificationService notificationService) : Hub
    {
        /// <summary>
        /// Sends a notification to the specified recipient.
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task SendNotification(NotificationDto notification)
        {
            await Clients.Group(notification.RecipientId.ToString()).SendAsync("ReceiveNotification", notification);
        }

        /// <summary>
        /// Adds the current connection to the group associated with the specified user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose group the connection should join.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task JoinGroup(int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        }

        /// <summary>
        /// Removes the current connection from the group associated with the specified user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose group the connection should leave.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task LeaveGroup(int userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
        }
    }
}