using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : GenericRepository<User>(context), IUserRepository
{
    /// <summary>
    /// Retrieves a User entity based on the provided username, including related entities.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>
    /// The User entity matching the provided username, including their posts, followers, and following, or null if no user is found.
    /// </returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await Context.Users
            .Include(u => u.Posts)
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// Retrieves a User entity based on the provided email, including related entities.
    /// </summary>
    /// <param name="email">The email to search for.</param>
    /// <returns>
    /// The User entity matching the provided email, including their posts, followers, and following, or null if no user is found.
    /// </returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Context.Users
            .Include(u => u.Posts)
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Checks if a user with the provided username exists in the database.
    /// </summary>
    /// <param name="username">The username to check for.</param>
    /// <returns>
    /// True if a user with the provided username exists, false otherwise.
    /// </returns>
    public async Task<bool> UserExistsAsync(string username)
    {
        return await Context.Users.AnyAsync(u => u.Username == username);
    }
    
    /// <summary>
    /// Checks if a user with the provided ID exists in the database.
    /// </summary>
    /// <param name="id">The ID of the user to check for.</param>
    /// <returns>
    /// True if a user with the provided ID exists, false otherwise.
    /// </returns>
    public async Task<bool> UserExistsAsync(int id)
    {
        return await Context.Users.AnyAsync(u => u.Id == id);
    }
}