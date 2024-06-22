using Octagram.Application.DTOs;

namespace Octagram.Application.Interfaces;

public interface ISearchService
{
    /// <summary>
    /// Searches for posts based on the given query.
    /// </summary>
    /// <param name="query">The search query to use.</param>
    /// <returns>A collection of post DTOs matching the search query.</returns>
    Task<IEnumerable<PostDto>> SearchPostsAsync(string query);

    /// <summary>
    /// Searches for users based on the given query.
    /// </summary>
    /// <param name="query">The search query to use.</param>
    /// <returns>A collection of user DTOs matching the search query.</returns>
    Task<IEnumerable<UserDto>> SearchUsersAsync(string query);

    /// <summary>
    /// Searches for hashtags based on the given query.
    /// </summary>
    /// <param name="query">The search query to use.</param>
    /// <returns>A collection of hashtag DTOs matching the search query.</returns>
    Task<IEnumerable<HashtagDto>> SearchHashtagsAsync(string query);
}