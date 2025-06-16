using System.Linq.Expressions;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<TEntity>> GetAllAsync(Guid userId);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Guid userId);

        Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(Guid userId, params Expression<Func<TEntity, object>>[] includes);
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        IQueryable<TEntity> Query();
    }
}
