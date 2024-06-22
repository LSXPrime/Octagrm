using Microsoft.AspNetCore.Mvc;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;

namespace Octagram.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController(ISearchService searchService) : ControllerBase
{
    /// <summary>
    /// Searches for users based on the provided query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>
    /// An Ok response with a list of matching UserDto objects.
    /// </returns>
    [HttpGet("users/{query}")]
    public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers(string query)
    {
        var users = await searchService.SearchUsersAsync(query);
        return Ok(users);
    }

    /// <summary>
    /// Searches for posts based on the provided query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>
    /// An Ok response with a list of matching PostDto objects.
    /// </returns>
    [HttpGet("posts/{query}")]
    public async Task<ActionResult<IEnumerable<PostDto>>> SearchPosts(string query)
    {
        var posts = await searchService.SearchPostsAsync(query);
        return Ok(posts);
    }

    /// <summary>
    /// Searches for hashtags based on the provided query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>
    /// An Ok response with a list of matching HashtagDto objects.
    /// </returns>
    [HttpGet("hashtags/{query}")]
    public async Task<ActionResult<IEnumerable<HashtagDto>>> SearchHashtags(string query)
    {
        var hashtags = await searchService.SearchHashtagsAsync(query);
        return Ok(hashtags);
    }
}