using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class CommentRepository(ApplicationDbContext context)
    : GenericRepository<Comment>(context), ICommentRepository
{
    /// <summary>
    /// Retrieves all comments associated with a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to retrieve comments for.</param>
    /// <returns>
    /// An enumerable collection of comments belonging to the specified post, including the associated user.
    /// </returns>
    public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId)
    {
        return await Context.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }
}