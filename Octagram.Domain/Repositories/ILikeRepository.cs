using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface ILikeRepository : IGenericRepository<Like>
{
    /// <summary>
    /// Retrieves a like based on the user ID and post ID.
    /// </summary>
    /// <param name="userId">The ID of the user who liked the post.</param>
    /// <param name="postId">The ID of the post that was liked.</param>
    /// <returns>
    /// The like entity if found, otherwise null.
    /// </returns>
    Task<Like> GetLikeByUserAndPostIdAsync(int userId, int postId);
    
    /// <summary>
    /// Retrieves the total number of likes for a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to count likes for.</param>
    /// <returns>
    /// The number of likes associated with the post.
    /// </returns>
    Task<int> GetLikeCountForPostAsync(int postId);
}