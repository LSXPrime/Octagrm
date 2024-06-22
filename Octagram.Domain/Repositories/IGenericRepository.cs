using System.Linq.Expressions;

namespace Octagram.Domain.Repositories;

public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>
    /// The entity with the specified ID, or null if no such entity exists.
    /// </returns>
    Task<T?> GetByIdAsync(int id);
    
    /// <summary>
    /// Retrieves all entities of the specified type.
    /// </summary>
    /// <returns>
    /// An IQueryable collection of all entities of the specified type.
    /// </returns>
    IQueryable<T> GetAllAsync();
    
    /// <summary>
    /// Retrieves entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities by.</param>
    /// <returns>
    /// An IQueryable collection of entities that satisfy the predicate.
    /// </returns>
    IQueryable<T> FindAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task<T> AddAsync(T entity);
    
    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task UpdateAsync(T entity);
    
    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task DeleteAsync(T entity);
}