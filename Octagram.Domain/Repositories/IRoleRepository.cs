using Octagram.Domain.Entities;

namespace Octagram.Domain.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    /// <summary>
    /// Retrieves a role by its name.
    /// </summary>
    /// <param name="roleName">The name of the role to retrieve.</param>
    /// <returns>The role with the specified name, or null if not found.</returns>
    Task<Role?> GetRoleByNameAsync(string roleName);
    
    /// <summary>
    /// Retrieves roles by their names.
    /// </summary>
    /// <param name="roleNames">The names of the roles to retrieve.</param>
    /// <returns>A collection of roles with the specified names.</returns>
    Task<IEnumerable<Role>> GetRolesByNameAsync(IEnumerable<string> roleNames);
    
    /// <summary>
    /// Checks if a role exists by its name.
    /// </summary>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns>True if the role exists, false otherwise.</returns>
    Task<bool> RoleExistsAsync(string roleName);
}