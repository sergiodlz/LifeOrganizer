using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace LifeOrganizer.Data.UnitOfWorkPattern
{
    /// <summary>
    /// Coordinates repository operations and commits changes as a single unit.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets a repository for the specified entity type.
        /// </summary>
        IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Begins a new database transaction.
        ///     This allows multiple operations to be executed as a single transaction,
        ///     ensuring that either all operations succeed or none do.
        /// </summary>
        /// <returns></returns>
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
