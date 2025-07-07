using LifeOrganizer.Data.Entities;
using LifeOrganizer.Business.DTOs;

namespace LifeOrganizer.Business.Services;

public interface ITransactionService : IGenericService<Transaction, TransactionDto>
{
    new Task AddAsync(TransactionDto dto, CancellationToken cancellationToken = default);
    Task TransferAsync(TransferDto transferDto, Guid userId, CancellationToken cancellationToken = default);
}