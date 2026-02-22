namespace Shared.Domain.UnitOfWork;

/// <summary>
/// Defines a unit of work for managing transactions across repositories.
/// Implementations should handle database transactions and SaveChanges operations.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this unit of work to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Saves all changes made in this unit of work to the database (synchronous).
    /// </summary>
    int SaveChanges();
}
