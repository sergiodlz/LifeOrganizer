using System.Linq.Expressions;
using AutoMapper;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.UnitOfWorkPattern;

namespace LifeOrganizer.Business.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseEntityDto
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GenericService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TDto?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            TEntity? dbEntity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id, userId);
            return dbEntity == null ? null : _mapper.Map<TDto>(dbEntity);
        }

        public async Task<IEnumerable<TDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            IEnumerable<TEntity> dbEntities = await _unitOfWork.Repository<TEntity>().GetAllAsync(userId);
            return _mapper.Map<IEnumerable<TDto>>(dbEntities);
        }

        public async Task<IEnumerable<TDto>> FindAsync(Expression<Func<TEntity, bool>> predicate, Guid userId, CancellationToken cancellationToken = default)
        {
            IEnumerable<TEntity> dbEntities = await _unitOfWork.Repository<TEntity>().FindAsync(predicate, userId);
            return _mapper.Map<IEnumerable<TDto>>(dbEntities);
        }

        public async Task AddAsync(TDto entity, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.Repository<TEntity>().AddAsync(_mapper.Map<TEntity>(entity));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(TDto entity, CancellationToken cancellationToken = default)
        {
            _unitOfWork.Repository<TEntity>().Update(_mapper.Map<TEntity>(entity));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(TDto entity, CancellationToken cancellationToken = default)
        {
            _unitOfWork.Repository<TEntity>().Remove(_mapper.Map<TEntity>(entity));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
