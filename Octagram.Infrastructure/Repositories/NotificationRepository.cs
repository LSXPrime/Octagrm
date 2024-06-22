using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class NotificationRepository(ApplicationDbContext context)
    : GenericRepository<Notification>(context), INotificationRepository
{
    /// <summary>
    /// Retrieves all notifications associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve notifications for.</param>
    /// <returns>
    /// An enumerable collection of notifications belonging to the specified user, including the associated sender.
    /// </returns>
    public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId)
    {
        return await Context.Notifications
            .Where(n => n.RecipientId == userId)
            .Include(n => n.Sender)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}