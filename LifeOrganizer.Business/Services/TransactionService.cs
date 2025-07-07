using AutoMapper;
using LifeOrganizer.Business.DTOs;
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
                        var existingTag = await tagRepo.GetByIdAsync(tag.Id, transaction.UserId);
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

        public async Task TransferAsync(TransferDto transferDto, Guid userId, CancellationToken cancellationToken = default)
        {
            var fromAccount = await _unitOfWork.Repository<Account>().GetByIdAsync(transferDto.AccountId, userId);
            var toAccount = await _unitOfWork.Repository<Account>().GetByIdAsync(transferDto.ToAccountId, userId);
            decimal toAccountAmount = transferDto.Amount;
            if (fromAccount == null || toAccount == null)
            {
                throw new ArgumentException("Invalid account IDs");
            }

            if (fromAccount.Currency != toAccount.Currency)
            {
                if (transferDto.AmountTo == null || transferDto.AmountTo <= 0)
                {
                    throw new ArgumentException("AmountTo must be provided and greater than zero for currency conversion");
                }

                toAccountAmount = transferDto.AmountTo.Value;
            }

            Transaction fromTransaction = new()
            {
                Amount = transferDto.Amount,
                Type = TransactionType.Expense,
                AccountId = fromAccount.Id,
                CategoryId = transferDto.CategoryId,
                SubcategoryId = transferDto.SubcategoryId,
                Description = $"Transfer to {toAccount.Name}",
                OccurredOn = DateTime.UtcNow,
                Currency = fromAccount.Currency,
                UserId = userId
            };

            Transaction toTransaction = new()
            {
                Amount = toAccountAmount,
                Type = TransactionType.Income,
                AccountId = toAccount.Id,
                CategoryId = transferDto.CategoryId,
                SubcategoryId = transferDto.SubcategoryId,
                Description = $"Transfer from {fromAccount.Name}",
                OccurredOn = DateTime.UtcNow,
                Currency = toAccount.Currency,
                UserId = userId
            };

            await _unitOfWork.Repository<Transaction>().AddAsync(fromTransaction);
            await _unitOfWork.Repository<Transaction>().AddAsync(toTransaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
