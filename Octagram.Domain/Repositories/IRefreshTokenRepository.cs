using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    /// <summary>
    /// Retrieves a RefreshToken entity by its token.
    /// </summary>
    /// <param name="token">The token of the RefreshToken entity to retrieve.</param>
    /// <returns>The RefreshToken entity with the specified token, or null if not found.</returns>
    Task<RefreshToken?> GetByTokenAsync(string token);

    /// <summary>
    /// Deletes all RefreshToken entities associated with a user.
    /// </summary>
    /// <param name="userId">The ID of the user whose RefreshToken entities should be deleted.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task DeleteByUserIdAsync(int userId);
}