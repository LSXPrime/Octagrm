using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface IPostRepository : IGenericRepository<Post>
{
    /// <summary>
    /// Retrieves all posts created by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve posts for.</param>
    /// <returns>
    /// An enumerable collection of posts created by the specified user.
    /// </returns>
    Task<IEnumerable<Post>> GetPostsByUserIdAsync(int userId);
    
    /// <summary>
    /// Gets a queryable collection of posts from users followed by the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user whose followed users' posts are to be retrieved.</param>
    /// <returns>
    /// A queryable collection of posts from users followed by the specified user.
    /// </returns>
    public IQueryable<Post> GetPostsFromFollowedUsers(int userId);
}