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
    [HttpGet("{userId:int}")]
    [AuthorizeMiddleware(true, "User")]
    public async Task<ActionResult<UserDto>> GetUserById(int userId)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound();

        var currentUserId = GetCurrentUserId();
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
    [AuthorizeMiddleware(true, "User")]
    public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
    {
        var user = await userService.GetUserByUsernameAsync(username);
        if (user == null)
            return NotFound();

        var currentUserId = GetCurrentUserId();
        user.IsFollowedByUser = await userService.IsFollowingAsync(currentUserId, user.Id);

        return Ok(user);
    }

    /// <summary>
    /// Update a user's profile information.
    /// </summary>
    /// <param name="request">The updated user data.</param>
    /// <returns>
    /// Returns an OK response with the updated user data if the update is successful.
    /// Returns a BadRequest response with the ModelState if the request is invalid.
    /// </returns>
    /// <remarks>
    /// This endpoint requires authentication and authorization as a "User."
    /// </remarks>
    [HttpPut]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var userId = GetCurrentUserId();
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
    /// <param name="followingId">The ID of the user to follow.</param>
    /// <returns>
    /// Returns an OK response with a message "User followed successfully." if the follow operation is successful.
    /// </returns>
    /// <remarks>
    /// This endpoint requires authentication and authorization as a "User."
    /// </remarks>
    [HttpPost("follow/{followingId:int}")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> FollowUser(int followingId)
    {
        var userId = GetCurrentUserId();
        if (userId == followingId)
            return BadRequest("You cannot follow yourself.");
        
        await userService.FollowUserAsync(userId, followingId);
        return Ok("User followed successfully.");
    }

    /// <summary>
    /// Unfollow a user.
    /// </summary>
    /// <param name="followingId">The ID of the user to unfollow.</param>
    /// <returns>
    /// Returns an OK response with a message "User unfollowed successfully." if the unfollow operation is successful.
    /// </returns>
    /// <remarks>
    /// This endpoint requires authentication and authorization as a "User."
    /// </remarks>
    [HttpDelete("follow/{followingId}")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> UnfollowUser(int followingId)
    {
        var userId = GetCurrentUserId();
        if (userId == followingId)
            return BadRequest("You cannot unfollow yourself.");
        
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
    [HttpGet("{userId:int}/followers")]
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
    [HttpGet("{userId:int}/following")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetFollowing(int userId)
    {
        var following = await userService.GetFollowingAsync(userId);
        return Ok(following);
    }

    /// <summary>
    /// Gets the current user's ID.
    /// </summary>
    /// <returns>
    /// Returns the current user's ID.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user ID is not found in the claims.</exception>
    [NonAction]
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in claims.");
        }

        return userId;
    }
}