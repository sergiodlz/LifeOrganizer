using System;

namespace LifeOrganizer.Business.DTOs;

public class BudgetPeriodDto : BaseEntityDto
{
    public Guid BudgetId { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }

    public decimal ActualAmount { get; set; }

    public ICollection<BudgetPeriodTransactionDto> BudgetPeriodTransactions { get; set; } = new List<BudgetPeriodTransactionDto>();
}
