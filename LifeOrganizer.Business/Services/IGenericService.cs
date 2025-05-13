using System.Linq.Expressions;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.Services
{
    public interface IGenericService<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
