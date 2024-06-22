using Octagram.Application.DTOs;

namespace Octagram.Application.Interfaces;

public interface IPostService
{
    /// <summary>
    /// Retrieves a paginated list of all posts, optionally filtered by hashtag.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of posts per page.</param>
    /// <param name="hashtag">The hashtag to filter posts by, if any.</param>
    /// <returns>A paginated result containing a collection of post DTOs.</returns>
    Task<PagedResult<PostDto>> GetAllPostsAsync(int page, int pageSize, string? hashtag = null);

    /// <summary>
    /// Retrieves a list of posts created by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose posts are to be retrieved.</param>
    /// <returns>A collection of post DTOs representing the user's posts.</returns>
    Task<IEnumerable<PostDto>> GetPostsByUserIdAsync(int userId);

    /// <summary>
    /// Retrieves a paginated list of posts created by users that the specified user is following.
    /// </summary>
    /// <param name="userId">The ID of the user whose following list will be used to retrieve posts.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of posts per page.</param>
    /// <returns>A paginated result containing a collection of post DTOs.</returns>
    Task<PagedResult<PostDto>> GetPostsFromFollowedUsersAsync(int userId, int page, int pageSize);

    /// <summary>
    /// Retrieves a specific post by its ID.
    /// </summary>
    /// <param name="postId">The ID of the post to retrieve.</param>
    /// <returns>The post DTO representing the specified post.</returns>
    Task<PostDto> GetPostByIdAsync(int postId);

    /// <summary>
    /// Creates a new post for the specified user.
    /// </summary>
    /// <param name="request">The request containing the post details.</param>
    /// <param name="userId">The ID of the user creating the post.</param>
    /// <returns>The newly created post DTO.</returns>
    Task<PostDto> CreatePostAsync(CreatePostRequest request, int userId);

    /// <summary>
    /// Updates an existing post created by the specified user.
    /// </summary>
    /// <param name="postId">The ID of the post to update.</param>
    /// <param name="request">The request containing the updated post details.</param>
    /// <param name="userId">The ID of the user updating the post.</param>
    /// <returns>The updated post DTO.</returns>
    Task<PostDto> UpdatePostAsync(int postId, UpdatePostRequest request, int userId);

    /// <summary>
    /// Deletes a post created by the specified user.
    /// </summary>
    /// <param name="postId">The ID of the post to delete.</param>
    /// <param name="userId">The ID of the user deleting the post.</param>
    Task DeletePostAsync(int postId, int userId);

    /// <summary>
    /// Allows a user to like a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to like.</param>
    /// <param name="userId">The ID of the user liking the post.</param>
    /// <returns>The like DTO representing the user's like action.</returns>
    Task<LikeDto> LikePostAsync(int postId, int userId);

    /// <summary>
    /// Allows a user to unlike a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to unlike.</param>
    /// <param name="userId">The ID of the user unliking the post.</param>
    Task UnlikePostAsync(int postId, int userId);

    /// <summary>
    /// Creates a new comment on a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to comment on.</param>
    /// <param name="request">The request containing the comment details.</param>
    /// <param name="userId">The ID of the user creating the comment.</param>
    /// <returns>The newly created comment DTO.</returns>
    Task<CommentDto> CreateCommentAsync(int postId, CreateCommentRequest request, int userId);

    /// <summary>
    /// Deletes a comment created by the specified user.
    /// </summary>
    /// <param name="commentId">The ID of the comment to delete.</param>
    /// <param name="userId">The ID of the user deleting the comment.</param>
    Task DeleteCommentAsync(int commentId, int userId);

    /// <summary>
    /// Retrieves a list of posts associated with a specific hashtag.
    /// </summary>
    /// <param name="hashtagName">The name of the hashtag to filter posts by.</param>
    /// <returns>A collection of post DTOs associated with the specified hashtag.</returns>
    Task<IEnumerable<PostDto>> GetPostsByHashtagAsync(string hashtagName);
}