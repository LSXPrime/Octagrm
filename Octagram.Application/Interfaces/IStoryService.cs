using Octagram.Application.DTOs;

namespace Octagram.Application.Interfaces;

public interface IStoryService
{
    /// <summary>
    /// Retrieves a story by its ID.
    /// </summary>
    /// <param name="id">The ID of the story to be retrieved.</param>
    /// <returns>The story DTO representing the requested story.</returns>
    Task<StoryDto> GetStoryByIdAsync(int id);
    
    /// <summary>
    /// Retrieves a list of stories created by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose stories are to be retrieved.</param>
    /// <returns>A collection of story DTOs representing the user's stories.</returns>
    Task<IEnumerable<StoryDto>> GetStoriesByUserIdAsync(int userId);

    /// <summary>
    /// Retrieves a list of stories created by users that the specified user is following.
    /// </summary>
    /// <param name="userId">The ID of the user whose following list will be used to retrieve stories.</param>
    /// <returns>A collection of story DTOs representing stories from the user's following list.</returns>
    Task<IEnumerable<StoryDto>> GetStoriesFromFollowingUsersAsync(int userId);

    /// <summary>
    /// Creates a new story for the specified user.
    /// </summary>
    /// <param name="request">The request containing the story details.</param>
    /// <param name="userId">The ID of the user creating the story.</param>
    /// <returns>The newly created story DTO.</returns>
    Task<StoryDto> CreateStoryAsync(CreateStoryRequest request, int userId);

    /// <summary>
    /// Deletes a story created by the specified user.
    /// </summary>
    /// <param name="storyId">The ID of the story to be deleted.</param>
    /// <param name="userId">The ID of the user deleting the story.</param>
    Task DeleteStoryAsync(int storyId, int userId);
}