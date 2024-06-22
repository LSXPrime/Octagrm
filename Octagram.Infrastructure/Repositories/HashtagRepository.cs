using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class HashtagRepository(ApplicationDbContext context)
    : GenericRepository<Hashtag>(context), IHashtagRepository
{
    /// <summary>
    /// Retrieves a hashtag by its name.
    /// </summary>
    /// <param name="name">The name of the hashtag to retrieve.</param>
    /// <returns>
    /// The Hashtag entity with the specified name, or null if no such hashtag exists.
    /// </returns>
    public async Task<Hashtag?> GetHashtagByNameAsync(string name)
    {
        return await Context.Hashtags.FirstOrDefaultAsync(h => h.Name == name);
    }
}