using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class LikeRepository(ApplicationDbContext context) : GenericRepository<Like>(context), ILikeRepository
{
    /// <summary>
    /// Retrieves a like based on the user ID and post ID.
    /// </summary>
    /// <param name="userId">The ID of the user who liked the post.</param>
    /// <param name="postId">The ID of the post that was liked.</param>
    /// <returns>
    /// The like entity if found, otherwise null.
    /// </returns>
    public async Task<Like> GetLikeByUserAndPostIdAsync(int userId, int postId)
    {
        return (await Context.Likes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId))!;
    }

    /// <summary>
    /// Retrieves the total number of likes for a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to count likes for.</param>
    /// <returns>
    /// The number of likes associated with the post.
    /// </returns>
    public async Task<int> GetLikeCountForPostAsync(int postId)
    {
        return await Context.Likes
            .CountAsync(l => l.PostId == postId);
    }
}