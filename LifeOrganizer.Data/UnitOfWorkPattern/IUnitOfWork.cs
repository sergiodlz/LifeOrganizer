using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.Repositories;

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
    }
}
