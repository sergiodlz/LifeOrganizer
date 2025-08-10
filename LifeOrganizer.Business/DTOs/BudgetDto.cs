using System;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.DTOs;

public class BudgetDto : BaseEntityDto
{
    public required string Name { get; set; }
    public decimal Amount { get; set; }
    public CurrencyType Currency { get; set; } = CurrencyType.USD;
    public ICollection<BudgetRuleDto> Rules { get; set; } = new List<BudgetRuleDto>();
    public ICollection<BudgetPeriodDto> Periods { get; set; } = new List<BudgetPeriodDto>();
}
