using System;

namespace LifeOrganizer.Data.Entities;

public class BudgetPeriod : BaseEntity
{
    public Guid BudgetId { get; set; }
    public Budget Budget { get; set; } = null!;

    public int Year { get; set; }
    public int Month { get; set; }

    public decimal ActualAmount { get; set; }
    
    public ICollection<BudgetPeriodTransaction> BudgetPeriodTransactions { get; set; } = new List<BudgetPeriodTransaction>();
}
