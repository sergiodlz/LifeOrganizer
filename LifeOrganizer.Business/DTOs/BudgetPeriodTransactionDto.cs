using System;

namespace LifeOrganizer.Business.DTOs;

public class BudgetPeriodTransactionDto : BaseEntityDto
{
    public Guid BudgetPeriodId { get; set; }
    public BudgetPeriodDto BudgetPeriod { get; set; } = null!;

    public Guid TransactionId { get; set; }
    public TransactionDto Transaction { get; set; } = null!;

    public decimal Amount { get; set; }
}
