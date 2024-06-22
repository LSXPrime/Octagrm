using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class FollowRepository(ApplicationDbContext context) : GenericRepository<Follow>(context), IFollowRepository
{
    /// <summary>
    /// Retrieves the follow relationship between two users, if it exists.
    /// </summary>
    /// <param name="followerId">The ID of the follower user.</param>
    /// <param name="followingId">The ID of the user being followed.</param>
    /// <returns>
    /// A `Follow` object representing the follow relationship, or `null` if no relationship exists.
    /// </returns>
    public async Task<Follow?> GetFollowRelationshipAsync(int followerId, int followingId)
    {
        return await Context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }

    /// <summary>
    /// Checks if a user is following another user.
    /// </summary>
    /// <param name="followerId">The ID of the follower user.</param>
    /// <param name="followingId">The ID of the user being followed.</param>
    /// <returns>
    /// `true` if the user is following the other user, `false` otherwise.
    /// </returns>
    public async Task<bool> IsFollowingAsync(int followerId, int followingId)
    {
        return await Context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }

    /// <summary>
    /// Retrieves all users following a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve followers for.</param>
    /// <returns>
    /// An enumerable collection of users following the specified user.
    /// </returns>
    public async Task<IEnumerable<User>> GetFollowersAsync(int userId)
    {
        return await Context.Follows
            .Where(f => f.FollowingId == userId)
            .Select(f => f.Follower)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all users that a specific user is following.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve following users for.</param>
    /// <returns>
    /// An enumerable collection of users that the specified user is following.
    /// </returns>
    public async Task<IEnumerable<User>> GetFollowingAsync(int userId)
    {
        return await Context.Follows
            .Where(f => f.FollowerId == userId)
            .Select(f => f.Following)
            .ToListAsync();
    }
}