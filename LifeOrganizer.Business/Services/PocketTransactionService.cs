using AutoMapper;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.UnitOfWorkPattern;
using Microsoft.EntityFrameworkCore.Storage;

namespace LifeOrganizer.Business.Services;

public class PocketTransactionService : GenericService<PocketTransaction, PocketTransactionDto>, IPocketTransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PocketTransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public override async Task AddAsync(PocketTransactionDto dto, CancellationToken cancellationToken = default)
    {
        IDbContextTransaction? dbTransaction = null;
        try
        {
            var accountRepo = _unitOfWork.Repository<Account>();
            var pocketRepo = _unitOfWork.Repository<Pocket>();
            var pocketTransactionRepo = _unitOfWork.Repository<PocketTransaction>();
            dbTransaction = await _unitOfWork.BeginTransactionAsync();
            var entity = _mapper.Map<PocketTransaction>(dto);
            var pocket = await pocketRepo.GetByIdAsync(dto.PocketId, dto.UserId)
                ?? throw new InvalidOperationException($"Pocket with ID {dto.PocketId} not found.");
            var account = await accountRepo.GetByIdAsync(pocket.AccountId, dto.UserId)
                ?? throw new InvalidOperationException($"Account with ID {pocket.AccountId} not found.");

            account.Balance -= dto.Type == TransactionType.Income ? dto.Amount : -dto.Amount;
            accountRepo.Update(account);
            pocket.Balance += dto.Type == TransactionType.Income ? dto.Amount : -dto.Amount;
            pocketRepo.Update(pocket);

            await pocketTransactionRepo.AddAsync(entity);
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

    public override async Task UpdateAsync(PocketTransactionDto dto, CancellationToken cancellationToken = default)
    {
        IDbContextTransaction? dbTransaction = null;
        try
        {
            var accountRepo = _unitOfWork.Repository<Account>();
            var pocketRepo = _unitOfWork.Repository<Pocket>();
            var pocketTransactionRepo = _unitOfWork.Repository<PocketTransaction>();
            dbTransaction = await _unitOfWork.BeginTransactionAsync();

            var existingTransaction = await pocketTransactionRepo.GetByIdAsync(
                dto.Id,
                dto.UserId) ?? throw new ArgumentException($"PocketTransaction with ID {dto.Id} not found");

            var pocket = await pocketRepo.GetByIdAsync(dto.PocketId, dto.UserId)
                ?? throw new InvalidOperationException($"Pocket with ID {dto.PocketId} not found.");

            var account = await accountRepo.GetByIdAsync(pocket.AccountId, dto.UserId)
                ?? throw new InvalidOperationException($"Account with ID {pocket.AccountId} not found.");

            pocket.Balance -= existingTransaction.Type == TransactionType.Income ? existingTransaction.Amount : -existingTransaction.Amount;
            account.Balance += existingTransaction.Type == TransactionType.Income ? dto.Amount : -dto.Amount;

            pocket.Balance += dto.Type == TransactionType.Income ? dto.Amount : -dto.Amount;
            account.Balance -= dto.Type == TransactionType.Income ? dto.Amount : -dto.Amount;
            accountRepo.Update(account);

            _mapper.Map(dto, existingTransaction);

            pocketRepo.Update(pocket);
            pocketTransactionRepo.Update(existingTransaction);
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
}
