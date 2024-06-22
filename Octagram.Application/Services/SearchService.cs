using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Octagram.Application.DTOs;
using Octagram.Application.Interfaces;
using Octagram.Domain.Repositories;

namespace Octagram.Application.Services;

public class SearchService(
    IPostRepository postRepository,
    IUserRepository userRepository,
    IHashtagRepository hashtagRepository,
    IMapper mapper)
    : ISearchService
{
    /// <summary>
    /// Searches for posts based on the given query.
    /// </summary>
    /// <param name="query">The search query to use.</param>
    /// <returns>A collection of post DTOs matching the search query.</returns>
    public async Task<IEnumerable<PostDto>> SearchPostsAsync(string query)
    {
        var posts = await postRepository.FindAsync(p =>
                p.Caption != null && p.Caption.Contains(query))
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .Include(p => p.PostHashtags)
            .OrderByDescending(p => p.CreatedAt).ToListAsync();
        return mapper.Map<IEnumerable<PostDto>>(posts);
    }

    /// <summary>
    /// Searches for users based on the given query.
    /// </summary>
    /// <param name="query">The search query to use.</param>
    /// <returns>A collection of user DTOs matching the search query.</returns>
    public async Task<IEnumerable<UserDto>> SearchUsersAsync(string query)
    {
        var users = await userRepository.FindAsync(u =>
                u.Username.Contains(query))
            .Include(p => p.Followers)
            .Include(p => p.Following)
            .ToListAsync();
        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    /// <summary>
    /// Searches for hashtags based on the given query.
    /// </summary>
    /// <param name="query">The search query to use.</param>
    /// <returns>A collection of hashtag DTOs matching the search query.</returns>
    public async Task<IEnumerable<HashtagDto>> SearchHashtagsAsync(string query)
    {
        var hashtags = await hashtagRepository.FindAsync(h =>
                h.Name.Contains(query))
            .Include(h => h.PostHashtags)
            .ToListAsync();
        return mapper.Map<IEnumerable<HashtagDto>>(hashtags);
    }
}