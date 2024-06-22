using Octagram.Application.DTOs;

namespace Octagram.Application.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>The user with the specified ID, or null if no user is found.</returns>
    Task<UserDto?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>The user with the specified username, or null if no user is found.</returns>
    Task<UserDto?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="request">The updated user information.</param>
    /// <returns>The updated user information, or null if the update failed.</returns>
    Task<UserDto?> UpdateUserAsync(int userId, UpdateUserRequest request);

    /// <summary>
    /// Searches for users based on a given query.
    /// </summary>
    /// <param name="query">The search query to use.</param>
    /// <returns>A collection of users matching the search query.</returns>
    Task<IEnumerable<UserDto>> SearchUsersAsync(string query);

    /// <summary>
    /// Allows a user to follow another user.
    /// </summary>
    /// <param name="followerId">The ID of the user following another user.</param>
    /// <param name="followingId">The ID of the user being followed.</param>
    Task FollowUserAsync(int followerId, int followingId);

    /// <summary>
    /// Allows a user to unfollow another user.
    /// </summary>
    /// <param name="followerId">The ID of the user unfollowing another user.</param>
    /// <param name="followingId">The ID of the user being unfollowed.</param>
    Task UnfollowUserAsync(int followerId, int followingId);

    /// <summary>
    /// Checks if a user is following another user.
    /// </summary>
    /// <param name="followerId">The ID of the user to check.</param>
    /// <param name="followingId">The ID of the user being checked.</param>
    /// <returns>True if the user is following the other user, false otherwise.</returns>
    Task<bool> IsFollowingAsync(int followerId, int followingId);

    /// <summary>
    /// Retrieves the list of followers for a given user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve followers for.</param>
    /// <returns>A collection of users who are following the specified user.</returns>
    Task<IEnumerable<UserDto>> GetFollowersAsync(int userId);

    /// <summary>
    /// Retrieves the list of users a given user is following.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve following for.</param>
    /// <returns>A collection of users the specified user is following.</returns>
    Task<IEnumerable<UserDto>> GetFollowingAsync(int userId);
}