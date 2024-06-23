using AutoMapper;
using Octagram.Application.DTOs;
using Octagram.Application.Exceptions;
using Octagram.Application.Interfaces;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using UnauthorizedAccessException = Octagram.Application.Exceptions.UnauthorizedAccessException;

namespace Octagram.Application.Services;

public class StoryService(
    IStoryRepository storyRepository,
    IUserRepository userRepository,
    IMapper mapper,
    IImageHelper imageHelper,
    ICloudStorageHelper cloudStorageHelper)
    : IStoryService
{
    /// <summary>
    /// Retrieves a story by its ID.
    /// </summary>
    /// <param name="id">The ID of the story to be retrieved.</param>
    /// <returns>The story DTO representing the requested story.</returns>
    public async Task<StoryDto> GetStoryByIdAsync(int id)
    {
        var story = await storyRepository.GetByIdAsync(id);
        return mapper.Map<StoryDto>(story);
    }
    
    /// <summary>
    /// Retrieves a list of stories created by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose stories are to be retrieved.</param>
    /// <returns>A collection of story DTOs representing the user's stories.</returns>
    public async Task<IEnumerable<StoryDto>> GetStoriesByUserIdAsync(int userId)
    {
        var stories = await storyRepository.GetStoriesByUserIdAsync(userId);
        return mapper.Map<IEnumerable<StoryDto>>(stories);
    }

    /// <summary>
    /// Retrieves a list of stories created by users that the specified user is following.
    /// </summary>
    /// <param name="userId">The ID of the user whose following list will be used to retrieve stories.</param>
    /// <returns>A collection of story DTOs representing stories from the user's following list.</returns>
    public async Task<IEnumerable<StoryDto>> GetStoriesFromFollowingUsersAsync(int userId)
    {
        var stories = await storyRepository.GetStoriesFromFollowingUsersAsync(userId);
        return mapper.Map<IEnumerable<StoryDto>>(stories);
    }

    /// <summary>
    /// Creates a new story for the specified user.
    /// </summary>
    /// <param name="request">The request containing the story details.</param>
    /// <param name="userId">The ID of the user creating the story.</param>
    /// <returns>The newly created story DTO.</returns>
    public async Task<StoryDto> CreateStoryAsync(CreateStoryRequest request, int userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        // Determine media type and process accordingly (image or video)
        var mediaUrl = request.MediaType switch
        {
            "image" => await imageHelper.ProcessAndUploadImageAsync(request.MediaFile, "stories",
                cloudStorageHelper),
            "video" => throw new NotImplementedException("Video upload is not yet implemented."),
            _ => throw new BadRequestException("Invalid media type.")
        };

        var story = new Story
        {
            UserId = userId,
            MediaUrl = mediaUrl,
            MediaType = request.MediaType,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        await storyRepository.AddAsync(story);
        return mapper.Map<StoryDto>(story);
    }

    /// <summary>
    /// Deletes a story created by the specified user.
    /// </summary>
    /// <param name="storyId">The ID of the story to be deleted.</param>
    /// <param name="userId">The ID of the user deleting the story.</param>
    public async Task DeleteStoryAsync(int storyId, int userId)
    {
        var story = await storyRepository.GetByIdAsync(storyId);
        if (story == null)
        {
            throw new NotFoundException("Story not found.");
        }

        if (story.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this story.");
        }

        await storyRepository.DeleteAsync(story);
    }
}