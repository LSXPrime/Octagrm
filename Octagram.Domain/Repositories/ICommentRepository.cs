using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface ICommentRepository : IGenericRepository<Comment>
{
    /// <summary>
    /// Retrieves a list of comments associated with a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to retrieve comments for.</param>
    /// <returns>An enumerable collection of comments associated with the specified post.</returns>
    Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);
}