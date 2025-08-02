using System;

namespace LifeOrganizer.Business.DTOs;

public class BudgetPeriodDto : BaseEntityDto
{
    public Guid BudgetId { get; set; } // Links to the main budget
    public BudgetDto Budget { get; set; } = null!; 

    public int Year { get; set; } // e.g. 2025
    public int Month { get; set; } // e.g. 1 for January

    public decimal ActualAmount { get; set; } // Sum of all matched transactions
}
