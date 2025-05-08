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

        public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Repository<TEntity>().GetAllAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Repository<TEntity>().FindAsync(predicate);
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
            _unitOfWork.Repository<TEntity>().Remove(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<TEntity> Query()
        {
            return _unitOfWork.Repository<TEntity>().Query();
        }
    }
}
