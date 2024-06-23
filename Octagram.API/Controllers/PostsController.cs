using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;
using Octagram.API.Attributes;

namespace Octagram.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController(IPostService postService) : ControllerBase
{
    /// <summary>
    /// Gets all posts with pagination and optional hashtag filter.
    /// </summary>
    /// <param name="page">The page number to retrieve (default: 1).</param>
    /// <param name="pageSize">The number of posts per page (default: 10).</param>
    /// <param name="hashtag">Optional hashtag to filter posts by.</param>
    /// <returns>
    /// A paged result containing the requested posts.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<PostDto>>> GetAllPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? hashtag = null
    )
    {
        var posts = await postService.GetAllPostsAsync(page, pageSize, hashtag);
        return Ok(posts);
    }

    /// <summary>
    /// Gets posts from followed users for the current user with pagination.
    /// </summary>
    /// <param name="page">The page number to retrieve (default: 1).</param>
    /// <param name="pageSize">The number of posts per page (default: 10).</param>
    /// <returns>
    /// A paged result containing the requested feed posts.
    /// </returns>
    [HttpGet("feed")]
    [AuthorizeMiddleware("User")]
    public async Task<ActionResult<PagedResult<PostDto>>> GetFeedPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var userId = GetCurrentUserId();
        var posts = await postService.GetPostsFromFollowedUsersAsync(userId, page, pageSize);
        return Ok(posts);
    }

    /// <summary>
    /// Gets a specific post by its ID.
    /// </summary>
    /// <param name="postId">The ID of the post to retrieve.</param>
    /// <returns>
    /// The requested post, or a NotFound result if the post is not found.
    /// </returns>
    [HttpGet("{postId:int}")]
    public async Task<ActionResult<PostDto>> GetPostById(int postId)
    {
        var post = await postService.GetPostByIdAsync(postId);
        return Ok(post);
    }

    /// <summary>
    /// Gets all posts created by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve posts for.</param>
    /// <returns>
    /// A collection of posts created by the specified user.
    /// </returns>
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsByUserId(int userId)
    {
        var posts = await postService.GetPostsByUserIdAsync(userId);
        return Ok(posts);
    }

    /// <summary>
    /// Creates a new post. Requires authentication.
    /// </summary>
    /// <param name="request">The details of the new post to create.</param>
    /// <returns>
    /// The newly created post.
    /// </returns>
    [HttpPost]
    [AuthorizeMiddleware("User")]
    public async Task<ActionResult<PostDto>>
        CreatePost([FromForm] CreatePostRequest request)
    {
        var userId = GetCurrentUserId();
        var post = await postService.CreatePostAsync(request, userId);
        return CreatedAtAction(nameof(GetPostById), new { postId = post.Id }, post);
    }

    /// <summary>
    /// Updates the caption of an existing post. Requires authentication and post ownership.
    /// </summary>
    /// <param name="postId">The ID of the post to update.</param>
    /// <param name="request">The updated caption for the post.</param>
    /// <returns>
    /// The updated post.
    /// </returns>
    [HttpPut("{postId:int}")]
    [AuthorizeMiddleware("User")]
    public async Task<ActionResult<PostDto>> UpdatePost(int postId, [FromBody] UpdatePostRequest request)
    {  
        var userId = GetCurrentUserId();
        var post = await postService.UpdatePostAsync(postId, request, userId);
        return Ok(post);
    }

    /// <summary>
    /// Deletes an existing post. Requires authentication and post ownership.
    /// </summary>
    /// <param name="postId">The ID of the post to delete.</param>
    /// <returns>
    /// A NoContent result if the post was successfully deleted.
    /// </returns>
    [HttpDelete("{postId:int}")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> DeletePost(int postId)
    {
        var userId = GetCurrentUserId();
        await postService.DeletePostAsync(postId, userId);
        return NoContent();
    }

    /// <summary>
    /// Likes a post. Requires authentication.
    /// </summary>
    /// <param name="postId">The ID of the post to like.</param>
    /// <returns>
    /// An OK result with a message indicating success.
    /// </returns>
    [HttpPost("{postId:int}/like")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> LikePost(int postId)
    {
        var userId = GetCurrentUserId();
        await postService.LikePostAsync(postId, userId);
        return Ok("Post liked successfully.");
    }

    /// <summary>
    /// Unlikes a post. Requires authentication.
    /// </summary>
    /// <param name="postId">The ID of the post to unlike.</param>
    /// <returns>
    /// An OK result with a message indicating success.
    /// </returns>
    [HttpDelete("{postId:int}/like")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> UnlikePost(int postId)
    {
        var userId = GetCurrentUserId();
        await postService.UnlikePostAsync(postId, userId);
        return Ok("Post unliked successfully.");
    }

    /// <summary>
    /// Creates a new comment on a post. Requires authentication.
    /// </summary>
    /// <param name="postId">The ID of the post to add the comment to.</param>
    /// <param name="request">The text of the comment to create.</param>
    /// <returns>
    /// The newly created comment.
    /// </returns>
    [HttpPost("{postId:int}/comments")]
    [AuthorizeMiddleware("User")]
    public async Task<ActionResult<CommentDto>> CreateComment(int postId, [FromBody] CreateCommentRequest request)
    {
        var userId = GetCurrentUserId();
        var comment = await postService.CreateCommentAsync(postId, request, userId);
        return CreatedAtAction(nameof(GetPostById), new { postId }, comment);
    }

    /// <summary>
    /// Deletes an existing comment. Requires authentication and comment ownership.
    /// </summary>
    /// <param name="postId">The ID of the post containing the comment.</param>
    /// <param name="commentId">The ID of the comment to delete.</param>
    /// <returns>
    /// A NoContent result if the comment was successfully deleted.
    /// </returns>
    /// <remarks>
    /// <see cref="postId"/> isn't required to be correct, it is only used to clarify the route.
    /// </remarks>
    [HttpDelete("{postId:int}/comments/{commentId:int}")]
    [AuthorizeMiddleware("User")]
    public async Task<IActionResult> DeleteComment(int postId, int commentId)
    {
        var userId = GetCurrentUserId();
        await postService.DeleteCommentAsync(commentId, userId);
        return NoContent();
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