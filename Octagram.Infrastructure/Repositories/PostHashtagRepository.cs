using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class PostHashtagRepository(ApplicationDbContext context)
    : GenericRepository<PostHashtag>(context), IPostHashtagRepository
{
    /// <summary>
    /// Adds a collection of hashtags to a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to add hashtags to.</param>
    /// <param name="hashtagIds">The collection of hashtag IDs to associate with the post.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddHashtagsToPostAsync(int postId, IEnumerable<int> hashtagIds)
    {
        foreach (var hashtagId in hashtagIds)
        {
            await Context.PostHashtags.AddAsync(new PostHashtag
            {
                PostId = postId,
                HashtagId = hashtagId
            });
        }
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves all posts associated with a specific hashtag.
    /// </summary>
    /// <param name="hashtagId">The ID of the hashtag to retrieve posts for.</param>
    /// <returns>An enumerable collection of posts associated with the specified hashtag, including the associated user.</returns>
    public async Task<IEnumerable<Post>> GetPostsByHashtagIdAsync(int hashtagId)
    {
        return await Context.PostHashtags
            .Where(ph => ph.HashtagId == hashtagId)
            .Select(ph => ph.Post)
            .Include(p => p.User)
            .ToListAsync();
    }
}