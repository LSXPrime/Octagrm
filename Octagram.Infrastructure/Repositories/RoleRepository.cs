using Microsoft.EntityFrameworkCore;
using Octagram.Domain.Entities;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.Infrastructure.Repositories;

public class RoleRepository(ApplicationDbContext context) : GenericRepository<Role>(context), IRoleRepository
{
    /// <summary>
    /// Retrieves a role by its name.
    /// </summary>
    /// <param name="roleName">The name of the role to retrieve.</param>
    /// <returns>The role with the specified name, or null if not found.</returns>
    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await Context.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }

    /// <summary>
    /// Retrieves roles by their names.
    /// </summary>
    /// <param name="roleNames">The names of the roles to retrieve.</param>
    /// <returns>A collection of roles with the specified names.</returns>
    public async Task<IEnumerable<Role>> GetRolesByNameAsync(IEnumerable<string> roleNames)
    {
        return await Context.Roles
            .Where(r => roleNames.Contains(r.Name))
            .ToListAsync();
    }
    
    /// <summary>
    /// Checks if a role exists by its name.
    /// </summary>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns>True if the role exists, false otherwise.</returns>
    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await Context.Roles
            .AnyAsync(r => r.Name == roleName);
    }
}