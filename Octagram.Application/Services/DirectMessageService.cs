using AutoMapper;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;

namespace Octagram.Application.Services;

public class DirectMessageService(IDirectMessageRepository directMessageRepository, IMapper mapper) : IDirectMessageService
{
    /// <summary>
    /// Sends a direct message between two users.
    /// </summary>
    /// <param name="request">The request containing the message details.</param>
    /// <param name="senderId">The ID of the user sending the message.</param>
    /// <returns>The newly created direct message DTO.</returns>
    public async Task<DirectMessageDto> SendDirectMessageAsync(CreateDirectMessageRequest request, int senderId)
    {
        var message = new DirectMessage
        {
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await directMessageRepository.AddAsync(message);
        return mapper.Map<DirectMessageDto>(message);
    }

    /// <summary>
    /// Retrieves all messages in a conversation between two users.
    /// </summary>
    /// <param name="userId1">The ID of the first user in the conversation.</param>
    /// <param name="userId2">The ID of the second user in the conversation.</param>
    /// <returns>A collection of direct message DTOs representing the conversation.</returns>
    public async Task<IEnumerable<DirectMessageDto>> GetConversationAsync(int userId1, int userId2)
    {
        var conversation = await directMessageRepository.GetConversationAsync(userId1, userId2);
        return mapper.Map<IEnumerable<DirectMessageDto>>(conversation);
    }
    

    /// <summary>
    /// Retrieves a paginated list of messages in a conversation between two users.
    /// </summary>
    /// <param name="userId1">The ID of the first user in the conversation.</param>
    /// <param name="userId2">The ID of the second user in the conversation.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of messages per page.</param>
    /// <returns>A paginated collection of direct message DTOs representing the conversation.</returns>
    public async Task<IEnumerable<DirectMessageDto>> GetConversationAsync(int userId1, int userId2, int page, int pageSize)
    {
        var conversation = await directMessageRepository.GetConversationAsync(userId1, userId2, page, pageSize);
        return mapper.Map<IEnumerable<DirectMessageDto>>(conversation);
    }

    /// <summary>
    /// Marks a direct message as read for a specific user.
    /// </summary>
    /// <param name="messageId">The ID of the message to mark as read.</param>
    /// <param name="userId">The ID of the user marking the message as read.</param>
    public async Task MarkMessageAsReadAsync(int messageId, int userId)
    {
        var message = await directMessageRepository.GetByIdAsync(messageId);

        // Ensure the user is either the sender or the receiver
        if (message?.SenderId != userId && message?.ReceiverId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to mark this message as read.");
        }

        message.IsRead = true;
        await directMessageRepository.UpdateAsync(message);
    }
}