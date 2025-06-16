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


        public async Task UpdateAsync(TDto dto, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<TEntity>();
            var trackedEntity = await repo.GetByIdAsync(dto.Id, dto.UserId);
            if (trackedEntity == null)
                throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} with Id {dto.Id} not found.");
            _mapper.Map(dto, trackedEntity); // Map changes onto tracked entity
            repo.Update(trackedEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(TDto dto, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<TEntity>();
            var trackedEntity = await repo.GetByIdAsync(dto.Id, dto.UserId);
            if (trackedEntity == null)
                throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} with Id {dto.Id} not found.");
            repo.Remove(trackedEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<TDto>> GetAllWithIncludesAsync(Guid userId, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            IEnumerable<TEntity> dbEntities = await _unitOfWork.Repository<TEntity>().GetAllWithIncludesAsync(userId, includes);
            return _mapper.Map<IEnumerable<TDto>>(dbEntities);
        }
    }
}
