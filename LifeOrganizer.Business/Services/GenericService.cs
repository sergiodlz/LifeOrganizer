using System.Linq.Expressions;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.UnitOfWorkPattern;

namespace LifeOrganizer.Business.Services
{
    public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : BaseEntity
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenericService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Repository<TEntity>().GetByIdAsync(id, userId);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Repository<TEntity>().GetAllAsync(userId);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Repository<TEntity>().FindAsync(predicate, userId);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.Repository<TEntity>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _unitOfWork.Repository<TEntity>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            // Soft delete: mark as deleted and update
            _unitOfWork.Repository<TEntity>().Remove(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
