using System;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.Services;

public interface IBudgetService : IGenericService<Budget, BudgetDto>
{
    Task EvaluateTransactionAsync(TransactionDto transaction);
}
