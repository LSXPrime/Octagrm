using Microsoft.AspNetCore.Mvc;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;
using Octagram.API.Attributes;
using System.Security.Claims;

namespace Octagram.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectMessageController(IDirectMessageService directMessageService) : ControllerBase
    {
        /// <summary>
        /// Gets the conversation between two users.
        /// </summary>
        /// <param name="userId1">The ID of the first user in the conversation.</param>
        /// <param name="userId2">The ID of the second user in the conversation.</param>
        /// <param name="page">The page number for pagination (default: 1).</param>
        /// <param name="pageSize">The number of messages per page (default: 10).</param>
        /// <returns>
        /// Returns an OK response with a list of DirectMessageDto objects representing the conversation.
        /// </returns>
        [HttpGet("{userId1}/{userId2}")]
        [AuthorizeMiddleware("User")]
        public async Task<ActionResult<IEnumerable<DirectMessageDto>>> GetConversation(int userId1, int userId2, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10) 
        {
            var conversation = await directMessageService.GetConversationAsync(userId1, userId2, page, pageSize);
            return Ok(conversation);
        }

        /// <summary>
        /// Sends a direct message.
        /// </summary>
        /// <param name="request">The request containing the message details.</param>
        /// <returns>
        /// Returns a CreatedAtAction response with the newly created DirectMessageDto object.
        /// </returns>
        /// <remarks>
        /// This function is not real-time and is intended for testing purposes. For real-time messaging, refer to the DirectMessagesHub.
        /// </remarks>
        [HttpPost]
        [AuthorizeMiddleware("User")]
        public async Task<ActionResult<DirectMessageDto>> SendDirectMessage([FromBody] CreateDirectMessageRequest request)
        {
            var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); 
            var message = await directMessageService.SendDirectMessageAsync(request, senderId);
            return CreatedAtAction(nameof(GetConversation), new { userId1 = senderId, userId2 = request.ReceiverId }, message);
        }

        /// <summary>
        /// Marks a message as read.
        /// </summary>
        /// <param name="messageId">The ID of the message to mark as read.</param>
        /// <returns>
        /// Returns an OK response with a message "Message marked as read."
        /// </returns>
        [HttpPatch("{messageId}/read")]
        [AuthorizeMiddleware("User")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await directMessageService.MarkMessageAsReadAsync(messageId, userId);
            return Ok("Message marked as read.");
        }
    }
}