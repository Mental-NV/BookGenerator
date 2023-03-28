using BookGenerator.Domain.Core;

namespace BookGenerator.Domain.Repositories;

public interface IChapterRepository
{
    /// <summary>
    /// Gets the entity with the specified identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>The maybe instance that may contain the entity with the specified identifier.</returns>
    Task<Chapter> GetByIdAsync(Guid id);

    /// <summary>
    /// Inserts the specified entity into the database.
    /// </summary>
    /// <param name="entity">The entity to be inserted into the database.</param>
    void Insert(Chapter entity);

    /// <summary>
    /// Updates the specified entity in the database.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    void Update(Chapter entity);
}
