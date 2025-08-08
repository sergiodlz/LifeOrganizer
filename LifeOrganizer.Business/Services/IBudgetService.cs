using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.Services;

public interface IBudgetService : IGenericService<Budget, BudgetDto>
{
    Task EvaluateTransactionAsync(TransactionDto transaction);
    Task ReEvaluateTransactionAsync(TransactionDto oldTransaction, TransactionDto newTransaction);
    Task RemoveTransactionEvaluationAsync(TransactionDto transaction);
}
