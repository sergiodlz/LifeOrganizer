
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.Services;

public interface ITransactionService : IGenericService<Transaction, TransactionDto>
{
    new Task AddAsync(TransactionDto dto, CancellationToken cancellationToken = default);
}