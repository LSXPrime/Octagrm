using Microsoft.AspNetCore.Mvc;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;
using Octagram.API.Attributes;
using System.Security.Claims;

namespace Octagram.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StoriesController(IStoryService storyService) : ControllerBase
{
    /// <summary>
    /// Gets a list of stories for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>
    /// An Ok response with a list of <see cref="StoryDto"/> objects.
    /// </returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<StoryDto>>> GetStoriesByUserId(int userId)
    {
        var stories = await storyService.GetStoriesByUserIdAsync(userId);
        return Ok(stories);
    }

    /// <summary>
    /// Gets a list of stories from users followed by the authenticated user.
    /// </summary>
    /// <returns>
    /// An Ok response with a list of <see cref="StoryDto"/> objects.
    /// </returns>
    [HttpGet("following")]
    [AuthorizeMiddleware("User")]
    public async Task<ActionResult<IEnumerable<StoryDto>>> GetStoriesFromFollowing()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var stories = await storyService.GetStoriesFromFollowingUsersAsync(userId);
        return Ok(stories);
    }

    /// <summary>
    /// Creates a new story for the authenticated user.
    /// </summary>
    /// <param name="request">The story creation request details.</param>
    /// <returns>
    /// A CreatedAtAction response with the newly created <see cref="StoryDto"/> object.
    /// Returns a BadRequest response with the ModelState if the request is invalid.
    /// </returns>
    [HttpPost]
    [AuthorizeMiddleware("User")]
    public async Task<ActionResult<StoryDto>> CreateStory([FromForm] CreateStoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var story = await storyService.CreateStoryAsync(request, userId);
        return CreatedAtAction(nameof(GetStoriesByUserId), new { userId = story.UserId }, story);
    }

    /// <summary>
    /// Deletes a story belonging to the authenticated user.
    /// </summary>
    /// <param name="storyId">The ID of the story to delete.</param>
    /// <returns>
    /// A NoContent response if the story is deleted successfully.
    /// </returns>
    [HttpDelete("{storyId}")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> DeleteStory(int storyId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await storyService.DeleteStoryAsync(storyId, userId);
        return NoContent();
    }
}