using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class DirectMessageRepository(ApplicationDbContext context)
    : GenericRepository<DirectMessage>(context), IDirectMessageRepository
{
    /// <summary>
    /// Retrieves all direct messages between two users.
    /// </summary>
    /// <param name="userId1">The ID of the first user.</param>
    /// <param name="userId2">The ID of the second user.</param>
    /// <returns>
    /// An enumerable collection of direct messages between the two specified users, including the associated sender and receiver.
    /// </returns>
    public async Task<IEnumerable<DirectMessage>> GetConversationAsync(int userId1, int userId2)
    {
        return await Context.DirectMessages
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }
    
    /// <summary>
    /// Retrieves a paginated list of direct messages between two users.
    /// </summary>
    /// <param name="userId1">The ID of the first user.</param>
    /// <param name="userId2">The ID of the second user.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of messages per page.</param>
    /// <returns>
    /// A paginated enumerable collection of direct messages between the two specified users, including the associated sender and receiver.
    /// </returns>
    public async Task<IEnumerable<DirectMessage>> GetConversationAsync(int userId1, int userId2, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;

        return await Context.DirectMessages
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderByDescending(m => m.CreatedAt) 
            .Skip(skip)
            .Take(pageSize)
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .ToListAsync();
    }
}