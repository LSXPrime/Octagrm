using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : GenericRepository<RefreshToken>(context), IRefreshTokenRepository
{
    
    /// <summary>
    /// Retrieves a RefreshToken entity by its token.
    /// </summary>
    /// <param name="token">The token of the RefreshToken entity to retrieve.</param>
    /// <returns>The RefreshToken entity with the specified token, or null if not found.</returns>
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await Context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    /// <summary>
    /// Deletes all RefreshToken entities associated with a user.
    /// </summary>
    /// <param name="userId">The ID of the user whose RefreshToken entities should be deleted.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task DeleteByUserIdAsync(int userId)
    {
        var tokensToDelete = await Context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
        Context.RefreshTokens.RemoveRange(tokensToDelete);
        await Context.SaveChangesAsync();
    }
}