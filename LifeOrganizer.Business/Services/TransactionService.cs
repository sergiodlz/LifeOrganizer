using AutoMapper;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.UnitOfWorkPattern;
using Microsoft.EntityFrameworkCore.Storage;

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
            IDbContextTransaction? dbTransaction = null;
            try
            {
                var accountRepo = _unitOfWork.Repository<Account>();
                var transactionRepo = _unitOfWork.Repository<Transaction>();
                dbTransaction = await _unitOfWork.BeginTransactionAsync();
                var entity = _mapper.Map<Transaction>(dto);
                var account = await accountRepo.GetByIdAsync(dto.AccountId, dto.UserId)
                    ?? throw new InvalidOperationException($"Account with ID {dto.AccountId} not found.");

                account.Balance += dto.Type == TransactionType.Income ? dto.Amount : -dto.Amount;
                accountRepo.Update(account);

                if (entity.Tags != null && entity.Tags.Count > 0)
                {
                    var tagRepo = _unitOfWork.Repository<Tag>();
                    var attachedTags = new List<Tag>();
                    foreach (var tag in entity.Tags)
                    {
                        if (tag.Id != Guid.Empty)
                        {
                            var existingTag = await tagRepo.GetByIdAsync(tag.Id, entity.UserId);
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
                    entity.Tags = attachedTags;
                }

                await transactionRepo.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                await dbTransaction.CommitAsync();
            }
            catch
            {
                if (dbTransaction != null)
                    await dbTransaction.RollbackAsync();
                throw;
            }
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

            TransactionDto fromTransaction = new()
            {
                Amount = transferDto.Amount,
                Type = TransactionType.Expense,
                AccountId = fromAccount.Id,
                CategoryId = transferDto.CategoryId,
                SubcategoryId = transferDto.SubcategoryId,
                Description = $"Transfer to {toAccount.Name}",
                OccurredOn = transferDto.OccurredOn,
                Currency = fromAccount.Currency,
                UserId = userId,
                Tags = transferDto.Tags
            };

            TransactionDto toTransaction = new()
            {
                Amount = toAccountAmount,
                Type = TransactionType.Income,
                AccountId = toAccount.Id,
                CategoryId = transferDto.CategoryId,
                SubcategoryId = transferDto.SubcategoryId,
                Description = $"Transfer from {fromAccount.Name}",
                OccurredOn = transferDto.OccurredOn,
                Currency = toAccount.Currency,
                UserId = userId,
                Tags = transferDto.Tags
            };

            await AddAsync(fromTransaction, cancellationToken);
            await AddAsync(toTransaction, cancellationToken);
        }

        public override async Task UpdateAsync(TransactionDto dto, CancellationToken cancellationToken = default)
        {
            IDbContextTransaction? dbTransaction = null;
            try
            {
                dbTransaction = await _unitOfWork.BeginTransactionAsync();
                var accountRepo = _unitOfWork.Repository<Account>();
                var transactionRepo = _unitOfWork.Repository<Transaction>();
                var tagRepo = _unitOfWork.Repository<Tag>();

                var existingTransaction = await transactionRepo.GetByIdWithIncludesAsync(
                    dto.Id,
                    dto.UserId,
                    t => t.Tags) ?? throw new ArgumentException($"Transaction with ID {dto.Id} not found");

                var oldAccount = await accountRepo.GetByIdAsync(existingTransaction.AccountId, dto.UserId)
                    ?? throw new InvalidOperationException($"Account with ID {existingTransaction.AccountId} not found.");
                var account = await accountRepo.GetByIdAsync(dto.AccountId, dto.UserId)
                    ?? throw new InvalidOperationException($"Account with ID {dto.AccountId} not found.");
                oldAccount.Balance -= existingTransaction.Type == TransactionType.Income ? existingTransaction.Amount : -existingTransaction.Amount;
                account.Balance += dto.Type == TransactionType.Income ? dto.Amount : -dto.Amount;

                _mapper.Map(dto, existingTransaction);
                existingTransaction.Tags = [];
                if (dto.Tags != null && dto.Tags.Count != 0)
                {
                    foreach (var tagDto in dto.Tags)
                    {
                        if (tagDto.Id != Guid.Empty)
                        {
                            var existingTag = await tagRepo.GetByIdAsync(tagDto.Id, existingTransaction.UserId);
                            if (existingTag != null)
                            {
                                existingTransaction.Tags.Add(existingTag);
                            }
                        }
                        else
                        {
                            var newTag = _mapper.Map<Tag>(tagDto);
                            existingTransaction.Tags.Add(newTag);
                        }
                    }
                }

                accountRepo.Update(oldAccount);
                accountRepo.Update(account);
                transactionRepo.Update(existingTransaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await dbTransaction.CommitAsync();
            }
            catch
            {
                if (dbTransaction != null)
                    await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public override async Task RemoveAsync(TransactionDto dto, CancellationToken cancellationToken = default)
        {
            IDbContextTransaction? dbTransaction = null;
            try
            {
                dbTransaction = await _unitOfWork.BeginTransactionAsync();                
                var accountRepo = _unitOfWork.Repository<Account>();
                var transactionRepo = _unitOfWork.Repository<Transaction>();
                var existingTransaction = await transactionRepo.GetByIdWithIncludesAsync(
                    dto.Id,
                    dto.UserId,
                    t => t.Tags) ?? throw new ArgumentException($"Transaction with ID {dto.Id} not found");

                var account = await accountRepo.GetByIdAsync(existingTransaction.AccountId, dto.UserId)
                    ?? throw new InvalidOperationException($"Account with ID {existingTransaction.AccountId} not found.");

                account.Balance -= existingTransaction.Type == TransactionType.Income ? existingTransaction.Amount : -existingTransaction.Amount;

                accountRepo.Update(account);
                transactionRepo.Remove(existingTransaction);
                await _unitOfWork.SaveChangesAsync();                
                await dbTransaction.CommitAsync();
            }
            catch
            {
                if (dbTransaction != null)
                    await dbTransaction.RollbackAsync();
                throw;
            }
        }
    }
}
