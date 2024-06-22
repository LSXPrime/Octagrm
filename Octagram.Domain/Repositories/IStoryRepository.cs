using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface IStoryRepository : IGenericRepository<Story>
{
    /// <summary>
    /// Retrieves all stories created by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve stories for.</param>
    /// <returns>
    /// An enumerable collection of stories created by the specified user.
    /// </returns>
    Task<IEnumerable<Story>> GetStoriesByUserIdAsync(int userId);
    
    /// <summary>
    /// Retrieves stories from users that the specified user is following.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve stories for.</param>
    /// <returns>
    /// An enumerable collection of stories created by users that the specified user follows.
    /// </returns>
    Task<IEnumerable<Story>> GetStoriesFromFollowingUsersAsync(int userId);
}