using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface IFollowRepository : IGenericRepository<Follow>
{
    /// <summary>
    /// Retrieves the follow relationship between two users, if it exists.
    /// </summary>
    /// <param name="followerId">The ID of the follower user.</param>
    /// <param name="followingId">The ID of the user being followed.</param>
    /// <returns>
    /// A `Follow` object representing the follow relationship, or `null` if no relationship exists.
    /// </returns>
    Task<Follow?> GetFollowRelationshipAsync(int followerId, int followingId);
    
    /// <summary>
    /// Checks if a user is following another user.
    /// </summary>
    /// <param name="followerId">The ID of the follower user.</param>
    /// <param name="followingId">The ID of the user being followed.</param>
    /// <returns>
    /// `true` if the user is following the other user, `false` otherwise.
    /// </returns>
    Task<bool> IsFollowingAsync(int followerId, int followingId);
    
    /// <summary>
    /// Retrieves all users following a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve followers for.</param>
    /// <returns>
    /// An enumerable collection of users following the specified user.
    /// </returns>
    Task<IEnumerable<User>> GetFollowersAsync(int userId);
    
    /// <summary>
    /// Retrieves all users that a specific user is following.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve following users for.</param>
    /// <returns>
    /// An enumerable collection of users that the specified user is following.
    /// </returns>
    Task<IEnumerable<User>> GetFollowingAsync(int userId);
}