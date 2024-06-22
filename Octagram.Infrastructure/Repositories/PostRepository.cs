using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class PostRepository(ApplicationDbContext context) : GenericRepository<Post>(context), IPostRepository
{
    /// <summary>
    /// Retrieves all posts created by a specific user, including associated user data, likes, and comments.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve posts for.</param>
    /// <returns>
    /// An enumerable collection of posts created by the specified user, including associated user data, likes, and comments, ordered by creation date (descending).
    /// </returns>
    public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(int userId)
    {
        return await Context.Posts
            .Where(p => p.UserId == userId)
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a queryable collection of posts from users followed by the specified user, including associated user data, likes, and comments.
    /// </summary>
    /// <param name="userId">The ID of the user whose followed users' posts are to be retrieved.</param>
    /// <returns>
    /// A queryable collection of posts from users followed by the specified user, including associated user data, likes, and comments, ordered by creation date (descending).
    /// </returns>
    public IQueryable<Post> GetPostsFromFollowedUsers(int userId)
    {
        return Context.Posts
            .Where(p => Context.Follows.Any(f => f.FollowerId == userId && f.FollowingId == p.UserId))
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt).AsQueryable();
    }
}