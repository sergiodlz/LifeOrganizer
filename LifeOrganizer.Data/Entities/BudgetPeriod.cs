using System;

namespace LifeOrganizer.Data.Entities;

public class BudgetPeriod : BaseEntity
{
    public Guid BudgetId { get; set; } // Links to the main budget
    public Budget Budget { get; set; } = null!; 

    public int Year { get; set; } // e.g. 2025
    public int Month { get; set; } // e.g. 1 for January

    public decimal ActualAmount { get; set; } // Sum of all matched transactions
}
