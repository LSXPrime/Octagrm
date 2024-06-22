using Microsoft.AspNetCore.SignalR;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;

namespace Octagram.API.Hubs;

public class DirectMessageHub(IDirectMessageService directMessageService) : Hub
{
    /// <summary>
    /// Sends a direct message to a specific user.
    /// </summary>
    /// <param name="request">The direct message request containing the message content and receiver ID.</param>
    /// <param name="senderId">The ID of the user sending the message.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SendMessage(CreateDirectMessageRequest request, int senderId)
    {
        var message = await directMessageService.SendDirectMessageAsync(request, senderId);

        // Send the message to the receiver's group
        await Clients.Group(request.ReceiverId.ToString()).SendAsync("ReceiveMessage", message);

        // Also send the message to the sender's group (for immediate display)
        await Clients.Group(senderId.ToString()).SendAsync("ReceiveMessage", message); 
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