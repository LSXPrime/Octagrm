using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface IDirectMessageRepository : IGenericRepository<DirectMessage>
{
    /// <summary>
    /// Retrieves all direct messages between two users.
    /// </summary>
    /// <param name="userId1">The ID of the first user.</param>
    /// <param name="userId2">The ID of the second user.</param>
    /// <returns>
    /// An enumerable collection of direct messages between the two specified users.
    /// </returns>
    Task<IEnumerable<DirectMessage>> GetConversationAsync(int userId1, int userId2);
    /// <summary>
    /// Retrieves a paginated list of direct messages between two users.
    /// </summary>
    /// <param name="userId1">The ID of the first user.</param>
    /// <param name="userId2">The ID of the second user.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of messages per page.</param>
    /// <returns>
    /// A paginated enumerable collection of direct messages between the two specified users.
    /// </returns>
    Task<IEnumerable<DirectMessage>> GetConversationAsync(int userId1, int userId2, int page, int pageSize);
}