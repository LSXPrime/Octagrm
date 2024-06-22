using Octagram.Application.DTOs;

namespace Octagram.Application.Interfaces;

public interface IDirectMessageService
{
    /// <summary>
    /// Sends a direct message between two users.
    /// </summary>
    /// <param name="request">The request containing the message details.</param>
    /// <param name="senderId">The ID of the user sending the message.</param>
    /// <returns>The newly created direct message DTO.</returns>
    Task<DirectMessageDto> SendDirectMessageAsync(CreateDirectMessageRequest request, int senderId);

    /// <summary>
    /// Retrieves all messages in a conversation between two users.
    /// </summary>
    /// <param name="userId1">The ID of the first user in the conversation.</param>
    /// <param name="userId2">The ID of the second user in the conversation.</param>
    /// <returns>A collection of direct message DTOs representing the conversation.</returns>
    Task<IEnumerable<DirectMessageDto>> GetConversationAsync(int userId1, int userId2);

    /// <summary>
    /// Retrieves a paginated list of messages in a conversation between two users.
    /// </summary>
    /// <param name="userId1">The ID of the first user in the conversation.</param>
    /// <param name="userId2">The ID of the second user in the conversation.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of messages per page.</param>
    /// <returns>A paginated collection of direct message DTOs representing the conversation.</returns>
    Task<IEnumerable<DirectMessageDto>> GetConversationAsync(int userId1, int userId2, int page, int pageSize);

    /// <summary>
    /// Marks a direct message as read for a specific user.
    /// </summary>
    /// <param name="messageId">The ID of the message to mark as read.</param>
    /// <param name="userId">The ID of the user marking the message as read.</param>
    Task MarkMessageAsReadAsync(int messageId, int userId);
}