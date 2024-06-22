using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;
using Octagram.API.Attributes;

namespace Octagram.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>
    /// Get a user's profile by ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>
    /// Returns an OK response with the user's profile data if the user is found.
    /// </returns>
    /// <remarks>
    /// This endpoint requires authentication and authorization as a "User."
    /// </remarks>
    [HttpGet("{userId}")]
    [AuthorizeMiddleware(true,"User")]
    public async Task<ActionResult<UserDto>> GetUserById(int userId)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user != null && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier)!, out var currentUserId))
            user.IsFollowedByUser = await userService.IsFollowingAsync(currentUserId, user.Id);

        return Ok(user);
    }

    /// <summary>
    /// Get a user's profile by username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>
    /// Returns an OK response with the user's profile data if the user is found.
    /// </returns>
    /// <remarks>
    /// This endpoint requires authentication and authorization as a "User."
    /// </remarks>
    [HttpGet("username/{username}")]
    [AuthorizeMiddleware(true,"User")]
    public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
    {
        var user = await userService.GetUserByUsernameAsync(username);
        if (user != null && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier)!, out var currentUserId))
            user.IsFollowedByUser = await userService.IsFollowingAsync(currentUserId, user.Id);

        return Ok(user);
    }

    /// <summary>
    /// Update a user's profile information.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="request">The updated user data.</param>
    /// <returns>
    /// Returns an OK response with the updated user data if the update is successful.
    /// Returns a BadRequest response with the ModelState if the request is invalid.
    /// </returns>
    /// <remarks>
    /// This endpoint requires authentication and authorization as a "User."
    /// </remarks>
    [HttpPut("{userId}")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await userService.UpdateUserAsync(userId, request);
        return Ok(user);
    }

    /// <summary>
    /// Search for users.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>
    /// Returns an OK response with a list of matching users.
    /// </returns>
    [HttpGet("search/{query}")]
    public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers(string query)
    {
        var users = await userService.SearchUsersAsync(query);
        return Ok(users);
    }

    /// <summary>
    /// Follow a user.
    /// </summary>
    /// <param name="userId">The ID of the user making the follow request.</param>
    /// <param name="followingId">The ID of the user to follow.</param>
    /// <returns>
    /// Returns an OK response with a message "User followed successfully." if the follow operation is successful.
    /// </returns>
    /// <remarks>
    /// This endpoint requires authentication and authorization as a "User."
    /// </remarks>
    [HttpPost("{userId}/follow/{followingId}")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> FollowUser(int userId, int followingId)
    {
        await userService.FollowUserAsync(userId, followingId);
        return Ok("User followed successfully.");
    }

    /// <summary>
    /// Unfollow a user.
    /// </summary>
    /// <param name="userId">The ID of the user making the unfollow request.</param>
    /// <param name="followingId">The ID of the user to unfollow.</param>
    /// <returns>
    /// Returns an OK response with a message "User unfollowed successfully." if the unfollow operation is successful.
    /// </returns>
    /// <remarks>
    /// This endpoint requires authentication and authorization as a "User."
    /// </remarks>
    [HttpDelete("{userId}/follow/{followingId}")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> UnfollowUser(int userId, int followingId)
    {
        await userService.UnfollowUserAsync(userId, followingId);
        return Ok("User unfollowed successfully.");
    }

    /// <summary>
    /// Get a user's followers.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve followers for.</param>
    /// <returns>
    /// Returns an OK response with a list of the user's followers.
    /// </returns>
    [HttpGet("{userId}/followers")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetFollowers(int userId)
    {
        var followers = await userService.GetFollowersAsync(userId);
        return Ok(followers);
    }

    /// <summary>
    /// Get a user's followed users.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve followed users for.</param>
    /// <returns>
    /// Returns an OK response with a list of the user's followed users.
    /// </returns>
    [HttpGet("{userId}/following")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetFollowing(int userId)
    {
        var following = await userService.GetFollowingAsync(userId);
        return Ok(following);
    }
}