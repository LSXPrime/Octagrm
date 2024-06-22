using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class StoryRepository(ApplicationDbContext context) : GenericRepository<Story>(context), IStoryRepository
{
    /// <summary>
    /// Retrieves all stories created by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve stories for.</param>
    /// <returns>
    /// An enumerable collection of stories created by the specified user.
    /// </returns>
    public async Task<IEnumerable<Story>> GetStoriesByUserIdAsync(int userId)
    {
        return await Context.Stories
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves stories from users that the specified user is following.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve stories for.</param>
    /// <returns>
    /// An enumerable collection of stories created by users that the specified user follows.
    /// </returns>
    public async Task<IEnumerable<Story>> GetStoriesFromFollowingUsersAsync(int userId)
    {
        return await Context.Stories
            .Where(s => Context.Follows.Any(f => f.FollowerId == userId && f.FollowingId == s.UserId))
            .Include(s => s.User)
            .ToListAsync();
    }
}