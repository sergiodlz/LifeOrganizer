using System;

namespace LifeOrganizer.Data.Entities;

public class BudgetPeriodTransaction : BaseEntity
{
    public Guid BudgetPeriodId { get; set; }
    public BudgetPeriod BudgetPeriod { get; set; } = null!;

    public Guid TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;

    public decimal Amount { get; set; }
}
