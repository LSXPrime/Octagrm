using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface INotificationRepository : IGenericRepository<Notification>
{
    /// <summary>
    /// Retrieves all notifications associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve notifications for.</param>
    /// <returns>
    /// An enumerable collection of notifications belonging to the specified user.
    /// </returns>
    Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId);
}