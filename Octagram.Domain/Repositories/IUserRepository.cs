using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Retrieves a User entity based on the provided username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>
    /// The User entity matching the provided username, or null if no user is found.
    /// </returns>
    Task<User?> GetByUsernameAsync(string username);
    
    /// <summary>
    /// Retrieves a User entity based on the provided email.
    /// </summary>
    /// <param name="email">The email to search for.</param>
    /// <returns>
    /// The User entity matching the provided email, or null if no user is found.
    /// </returns>
    Task<User?> GetByEmailAsync(string email);
    
    /// <summary>
    /// Checks if a user with the provided username exists in the database.
    /// </summary>
    /// <param name="username">The username to check for.</param>
    /// <returns>
    /// True if a user with the provided username exists, false otherwise.
    /// </returns>
    Task<bool> UserExistsAsync(string username);
    
    /// <summary>
    /// Checks if a user with the provided ID exists in the database.
    /// </summary>
    /// <param name="id">The ID of the user to check for.</param>
    /// <returns>
    /// True if a user with the provided ID exists, false otherwise.
    /// </returns>
    Task<bool> UserExistsAsync(int id);
}