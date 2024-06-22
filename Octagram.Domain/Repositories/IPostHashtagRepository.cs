using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface IPostHashtagRepository : IGenericRepository<PostHashtag>
{
    /// <summary>
    /// Adds a collection of hashtags to a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post to add hashtags to.</param>
    /// <param name="hashtagIds">The collection of hashtag IDs to associate with the post.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddHashtagsToPostAsync(int postId, IEnumerable<int> hashtagIds);
    
    /// <summary>
    /// Retrieves all posts associated with a specific hashtag.
    /// </summary>
    /// <param name="hashtagId">The ID of the hashtag to retrieve posts for.</param>
    /// <returns>An enumerable collection of posts associated with the specified hashtag.</returns>
    Task<IEnumerable<Post>> GetPostsByHashtagIdAsync(int hashtagId);
}