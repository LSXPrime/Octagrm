using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Octagram.Application.DTOs;
using Octagram.Application.Exceptions;
using Octagram.Application.Interfaces;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;

namespace Octagram.Application.Services;

public class UserService(IUserRepository userRepository, IFollowRepository followRepository, INotificationService notificationService, IMapper mapper)
    : IUserService
{
    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>The user with the specified ID, or null if no user is found.</returns>
    /// <exception cref="Exception">Thrown if the user is not found.</exception>
    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }
        return mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>The user with the specified username, or null if no user is found.</returns>
    /// <exception cref="NotFoundException">Thrown if the user is not found.</exception>
    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }
        return mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="request">The updated user information.</param>
    /// <returns>The updated user information, or null if the update failed.</returns>
    /// <exception cref="NotFoundException">Thrown if the user is not found.</exception>
    public async Task<UserDto?> UpdateUserAsync(int userId, UpdateUserRequest request)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        user.Username = string.IsNullOrEmpty(request.Username) ? user.Username : request.Username;
        user.Bio = string.IsNullOrEmpty(request.Bio) ? user.Bio : request.Bio;
        user.ProfileImageUrl = string.IsNullOrEmpty(request.ProfilePicture) ? user.ProfileImageUrl : request.ProfilePicture;
        
        await userRepository.UpdateAsync(user);
        return mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// Searches for users based on a given query.
    /// </summary>
    /// <param name="query">The search query to use.</param>
    /// <returns>A collection of users matching the search query.</returns>
    public async Task<IEnumerable<UserDto>> SearchUsersAsync(string query)
    {
        var users = await userRepository.FindAsync(u => 
            u.Username.Contains(query)).ToListAsync(); // Basic search implementation
        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    /// <summary>
    /// Allows a user to follow another user.
    /// </summary>
    /// <param name="followerId">The ID of the user following another user.</param>
    /// <param name="followingId">The ID of the user being followed.</param>
    /// <exception cref="NotFoundException">Thrown if either the follower or following user is not found.</exception>
    /// <exception cref="BadRequestException">Thrown if the follower is already following the user.</exception>
    public async Task FollowUserAsync(int followerId, int followingId)
    {
        // Check if users exist (you might want to move this to a separate method)
        var follower = await userRepository.UserExistsAsync(followerId);
        if (follower == false)
        {
            throw new NotFoundException("Follower not found.");
        }
        
        var following = await userRepository.UserExistsAsync(followingId);
        if (following == false)
        {
            throw new NotFoundException("Following not found.");
        }

        var existingFollow = await followRepository.GetFollowRelationshipAsync(followerId, followingId);
        if (existingFollow != null)
        {
            throw new BadRequestException("You are already following this user.");
        }

        var follow = new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId
        };
        
        await notificationService.CreateFollowNotificationAsync(follow.FollowerId, follow.FollowingId);

        await followRepository.AddAsync(follow);
    }

    /// <summary>
    /// Allows a user to unfollow another user.
    /// </summary>
    /// <param name="followerId">The ID of the user unfollowing another user.</param>
    /// <param name="followingId">The ID of the user being unfollowed.</param>
    /// <exception cref="NotFoundException">Thrown if either the follower or following user is not found.</exception>
    /// <exception cref="BadRequestException">Thrown if the follower is not already following the user.</exception>
    public async Task UnfollowUserAsync(int followerId, int followingId)
    {
        // Check if users exist 
        var follower = await userRepository.UserExistsAsync(followerId);
        if (follower == false)
        {
            throw new NotFoundException("Follower not found.");
        }
        
        var following = await userRepository.UserExistsAsync(followingId);
        if (following == false)
        {
            throw new NotFoundException("Following not found.");
        }

        var follow = await followRepository.GetFollowRelationshipAsync(followerId, followingId);
        if (follow == null)
        {
            throw new BadRequestException("You are not following this user.");
        }

        await followRepository.DeleteAsync(follow);
    }

    /// <summary>
    /// Checks if a user is following another user.
    /// </summary>
    /// <param name="followerId">The ID of the user to check.</param>
    /// <param name="followingId">The ID of the user being checked.</param>
    /// <returns>True if the user is following the other user, false otherwise.</returns>
    public async Task<bool> IsFollowingAsync(int followerId, int followingId)
    {
        return await followRepository.IsFollowingAsync(followerId, followingId);
    }

    /// <summary>
    /// Retrieves the list of followers for a given user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve followers for.</param>
    /// <returns>A collection of users who are following the specified user.</returns>
    public async Task<IEnumerable<UserDto>> GetFollowersAsync(int userId)
    {
        var followers = await followRepository.GetFollowersAsync(userId);
        return mapper.Map<IEnumerable<UserDto>>(followers);
    }

    /// <summary>
    /// Retrieves the list of users a given user is following.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve following for.</param>
    /// <returns>A collection of users the specified user is following.</returns>
    public async Task<IEnumerable<UserDto>> GetFollowingAsync(int userId)
    {
        var following = await followRepository.GetFollowingAsync(userId);
        return mapper.Map<IEnumerable<UserDto>>(following);
    }
}