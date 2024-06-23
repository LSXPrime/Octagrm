using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Octagram.API.Attributes;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;
using Octagram.Domain.Repositories;

namespace Octagram.API.Hubs;

public class DirectMessageHub(IDirectMessageService directMessageService, IUserRepository userRepository) : Hub
{
    /// <summary>
    /// Sends a direct message to a specific user.
    /// </summary>
    /// <param name="request">The direct message request containing the message content and receiver ID.</param>
    /// <param name="senderId">The ID of the user sending the message.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    [AuthorizeMiddleware("User")]
    public async Task SendMessage(CreateDirectMessageRequest request, int senderId)
    {
        // Check if the sender is the current user
        if (!int.TryParse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var currentUserId) ||senderId != currentUserId)
        {
            await Clients.Caller.SendAsync("ErrorMessage", "Invalid sender ID.");
            return;
        }

        // Check if the sender and receiver exist
        if (!await userRepository.UserExistsAsync(currentUserId) || !await userRepository.UserExistsAsync(request.ReceiverId))
        {
            await Clients.Caller.SendAsync("ErrorMessage", "Sender or receiver not found.");
            return;
        }
        
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
    [AuthorizeMiddleware("User")]
    public async Task JoinGroup(int userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
    }

    /// <summary>
    /// Removes the current connection from the group associated with the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose group the connection should leave.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    [AuthorizeMiddleware("User")]
    public async Task LeaveGroup(int userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
    }
}