using AutoMapper;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.UnitOfWorkPattern;

namespace LifeOrganizer.Business.Services
{
    public class TransactionService : GenericService<Transaction, TransactionDto>, ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public override async Task AddAsync(TransactionDto dto, CancellationToken cancellationToken = default)
        {
            var transaction = _mapper.Map<Transaction>(dto);

            // Attach existing tags by Id
            if (transaction.Tags != null && transaction.Tags.Count > 0)
            {
                var tagRepo = _unitOfWork.Repository<Tag>();
                var attachedTags = new List<Tag>();
                foreach (var tag in transaction.Tags)
                {
                    if (tag.Id != Guid.Empty)
                    {
                        var existingTag = await tagRepo.GetByIdAsync(tag.Id, tag.UserId);
                        if (existingTag != null)
                        {
                            attachedTags.Add(existingTag);
                        }
                    }
                    else
                    {
                        attachedTags.Add(tag);
                    }
                }
                transaction.Tags = attachedTags;
            }

            await _unitOfWork.Repository<Transaction>().AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
