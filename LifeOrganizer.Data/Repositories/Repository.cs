using System.Linq.Expressions;
using LifeOrganizer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeOrganizer.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly LifeOrganizerContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(LifeOrganizerContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }


        public virtual async Task<TEntity?> GetByIdAsync(Guid id, Guid userId)
        {
            // Only return if not soft deleted and owned by user
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted && e.UserId == userId);
        }


        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Guid userId)
        {
            // Only return entities not soft deleted and owned by user
            return await _dbSet.Where(e => !e.IsDeleted && e.UserId == userId).ToListAsync();
        }


        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Guid userId)
        {
            // Only return entities not soft deleted and owned by user
            return await _dbSet.Where(e => !e.IsDeleted && e.UserId == userId).Where(predicate).ToListAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            // Soft delete: mark as deleted and update
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }

        public virtual IQueryable<TEntity> Query()
        {
            // Only return entities not soft deleted
            return _dbSet.Where(e => !e.IsDeleted).AsQueryable();
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }
    }
}
