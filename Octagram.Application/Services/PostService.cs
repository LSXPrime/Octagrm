using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Octagram.Application.DTOs;
using Octagram.Application.Exceptions;
using Octagram.Application.Interfaces;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using UnauthorizedAccessException = Octagram.Application.Exceptions.UnauthorizedAccessException;

namespace Octagram.Application.Services;

public class PostService(
    IPostRepository postRepository,
    IUserRepository userRepository,
    ILikeRepository likeRepository,
    ICommentRepository commentRepository,
    IHashtagRepository hashtagRepository,
    IPostHashtagRepository postHashtagRepository,
    INotificationService notificationService,
    IMapper mapper,
    IImageHelper imageHelper,
    ICloudStorageHelper cloudStorageHelper)
    : IPostService
{
    /// <summary>
    /// Retrieves a paginated list of all posts, optionally filtered by hashtag.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of posts per page.</param>
    /// <param name="hashtag">The hashtag to filter posts by, if any.</param>
    /// <returns>A paginated result containing a collection of post DTOs.</returns>
    public async Task<PagedResult<PostDto>> GetAllPostsAsync(int page, int pageSize, string? hashtag = null)
    {
        var query = postRepository.GetAllAsync(); // Start with all posts

        if (!string.IsNullOrEmpty(hashtag))
        {
            var hashtagEntity = await hashtagRepository.GetHashtagByNameAsync(hashtag.ToLower());
            if (hashtagEntity != null)
            {
                query = postRepository.FindAsync(p => p.PostHashtags.Any(ph => ph.HashtagId == hashtagEntity.Id));
            }
        }

        // Apply pagination
        var skip = (page - 1) * pageSize;
        var posts = await query.Skip(skip).Take(pageSize).ToListAsync();

        // Calculate total count
        var totalCount = query.Count();

        return new PagedResult<PostDto>
        {
            Items = mapper.Map<IEnumerable<PostDto>>(posts),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Retrieves a list of posts created by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose posts are to be retrieved.</param>
    /// <returns>A collection of post DTOs representing the user's posts.</returns>
    public async Task<IEnumerable<PostDto>> GetPostsByUserIdAsync(int userId)
    {
        var posts = await postRepository.GetPostsByUserIdAsync(userId);
        return mapper.Map<IEnumerable<PostDto>>(posts);
    }

    /// <summary>
    /// Retrieves a paginated list of posts created by users that the specified user is following.
    /// </summary>
    /// <param name="userId">The ID of the user whose following list will be used to retrieve posts.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of posts per page.</param>
    /// <returns>A paginated result containing a collection of post DTOs.</returns>
    public async Task<PagedResult<PostDto>> GetPostsFromFollowedUsersAsync(int userId, int page, int pageSize)
    {
        var query = postRepository.GetPostsFromFollowedUsers(userId);

        // Apply pagination
        var skip = (page - 1) * pageSize;
        var posts = await query.Skip(skip).Take(pageSize).ToListAsync();

        // Calculate total count
        var totalCount = await query.CountAsync();
        
        return new PagedResult<PostDto>
        {
            Items = mapper.Map<IEnumerable<PostDto>>(posts),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Retrieves a specific post by its ID.
    /// </summary>
    /// <param name="postId">The ID of the post to retrieve.</param>
    /// <returns>The post DTO representing the specified post.</returns>
    public async Task<PostDto> GetPostByIdAsync(int postId)
    {
        var post = await postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new NotFoundException("Post not found.");
        }

        return mapper.Map<PostDto>(post);
    }

    /// <summary>
    /// Creates a new post for the specified user.
    /// </summary>
    /// <param name="request">The request containing the post details.</param>
    /// <param name="userId">The ID of the user creating the post.</param>
    /// <returns>The newly created post DTO.</returns>
    public async Task<PostDto> CreatePostAsync(CreatePostRequest request, int userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        var imageUrl = await imageHelper.ProcessAndUploadImageAsync(request.ImageFile, "posts", cloudStorageHelper);

        var post = new Post
        {
            ImageUrl = imageUrl,
            Caption = request.Caption,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await postRepository.AddAsync(post);

        if (request.Hashtags.Count != 0)
        {
            await AddHashtagsToPost(post, request.Hashtags);
        }

        return mapper.Map<PostDto>(post);
    }

    /// <summary>
    /// Updates an existing post created by the specified user.
    /// </summary>
    /// <param name="postId">The ID of the post to update.</param>
    /// <param name="request">The request containing the updated post details.</param>
    /// <param name="userId">The ID of the user updating the post.</param>
    /// <returns>The updated post DTO.</returns>
    public async Task<PostDto> UpdatePostAsync(int postId, UpdatePostRequest request, int userId)
    {
        var post = await postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new NotFoundException("Post not found.");
        }

        if (post.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this post.");
        }

        post.Caption = request.Caption;

        await postRepository.UpdateAsync(post);
        return mapper.Map<PostDto>(post);
    }

    /// <summary>
    /// Deletes a post created by the specified user.
    /// </summary>
    /// <param name="postId">The ID of the post to delete.</param>
    /// <param name="userId">The ID of the user deleting the post.</param>
    public async Task DeletePostAsync(int postId, int userId)
    {
        var post = await postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new NotFoundException("Post not found.");
        }

        if (post.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this post.");
        }

        await postRepository.DeleteAsync(post);
    }

    /// <summary>
    /// Allows a user to like a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to like.</param>
    /// <param name="userId">The ID of the user liking the post.</param>
    /// <returns>The like DTO representing the user's like action.</returns>
    public async Task<LikeDto> LikePostAsync(int postId, int userId)
    {
        var existingLike = await likeRepository.GetLikeByUserAndPostIdAsync(userId, postId);
        if (existingLike != null)
        {
            throw new BadRequestException("You already liked this post.");
        }

        var like = new Like
        {
            PostId = postId,
            UserId = userId
        };
        
        await notificationService.CreateLikeNotificationAsync(postId, userId);

        await likeRepository.AddAsync(like);
        return mapper.Map<LikeDto>(like);
    }

    /// <summary>
    /// Allows a user to unlike a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to unlike.</param>
    /// <param name="userId">The ID of the user unliking the post.</param>
    public async Task UnlikePostAsync(int postId, int userId)
    {
        var like = await likeRepository.GetLikeByUserAndPostIdAsync(userId, postId);
        if (like == null)
        {
            throw new BadRequestException("You haven't liked this post.");
        }

        await likeRepository.DeleteAsync(like);
    }

    /// <summary>
    /// Creates a new comment on a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to comment on.</param>
    /// <param name="request">The request containing the comment details.</param>
    /// <param name="userId">The ID of the user creating the comment.</param>
    /// <returns>The newly created comment DTO.</returns>
    public async Task<CommentDto> CreateCommentAsync(int postId, CreateCommentRequest request, int userId)
    {
        var post = await postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new NotFoundException("Post not found.");
        }

        var comment = new Comment
        {
            Content = request.Content,
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        await notificationService.CreateCommentNotificationAsync(comment.Id, userId);

        await commentRepository.AddAsync(comment);
        return mapper.Map<CommentDto>(comment);
    }

    /// <summary>
    /// Deletes a comment created by the specified user.
    /// </summary>
    /// <param name="commentId">The ID of the comment to delete.</param>
    /// <param name="userId">The ID of the user deleting the comment.</param>
    public async Task DeleteCommentAsync(int commentId, int userId)
    {
        var comment = await commentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            throw new NotFoundException("Comment not found.");
        }

        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this comment.");
        }

        await commentRepository.DeleteAsync(comment);
    }

    /// <summary>
    /// Retrieves a list of posts associated with a specific hashtag.
    /// </summary>
    /// <param name="hashtagName">The name of the hashtag to filter posts by.</param>
    /// <returns>A collection of post DTOs associated with the specified hashtag.</returns>
    public async Task<IEnumerable<PostDto>> GetPostsByHashtagAsync(string hashtagName)
    {
        var hashtag = await hashtagRepository.GetHashtagByNameAsync(hashtagName.ToLower());
        if (hashtag == null)
        {
            throw new NotFoundException("Hashtag not found.");
        }

        var posts = await postHashtagRepository.GetPostsByHashtagIdAsync(hashtag.Id);
        return mapper.Map<IEnumerable<PostDto>>(posts);
    }

    /// <summary>
    /// Adds a list of hashtags to a given post.
    /// </summary>
    /// <param name="post">The post to add hashtags to.</param>
    /// <param name="hashtagNames">The list of hashtag names to add.</param>
    private async Task AddHashtagsToPost(Post post, List<string> hashtagNames)
    {
        foreach (var hashtagName in hashtagNames)
        {
            var hashtag = await hashtagRepository.GetHashtagByNameAsync(hashtagName.ToLower()) ??
                          await hashtagRepository.AddAsync(new Hashtag { Name = hashtagName.ToLower() });

            await postHashtagRepository.AddAsync(new PostHashtag
            {
                PostId = post.Id,
                HashtagId = hashtag.Id
            });
        }
    }
}