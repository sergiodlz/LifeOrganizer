using System.Linq.Expressions;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.Services
{
    public interface IGenericService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseEntityDto
    {
        Task<TDto?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TDto>> FindAsync(Expression<Func<TEntity, bool>> predicate, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TDto>> GetAllWithIncludesAsync(Guid userId, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
        Task AddAsync(TDto entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TDto entity, CancellationToken cancellationToken = default);
        Task RemoveAsync(TDto entity, CancellationToken cancellationToken = default);
    }
}
