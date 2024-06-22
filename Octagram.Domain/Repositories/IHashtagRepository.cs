using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface IHashtagRepository : IGenericRepository<Hashtag>
{
    /// <summary>
    /// Retrieves a hashtag by its name.
    /// </summary>
    /// <param name="name">The name of the hashtag to retrieve.</param>
    /// <returns>
    /// The Hashtag entity with the specified name, or null if no such hashtag exists.
    /// </returns>
    Task<Hashtag?> GetHashtagByNameAsync(string name);
}