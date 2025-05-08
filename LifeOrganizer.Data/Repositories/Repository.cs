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


        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            // Only return if not soft deleted
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }


        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            // Only return entities not soft deleted
            return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
        }


        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            // Only return entities not soft deleted
            return await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
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
    }
}
