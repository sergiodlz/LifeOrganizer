namespace LifeOrganizer.Data.Entities;

public class Budget : BaseEntity
{
    public required string Name { get; set; }
    public decimal Amount { get; set; } // Monthly amount
    public CurrencyType Currency { get; set; } = CurrencyType.USD;

    public ICollection<BudgetRule> Rules { get; set; } = new List<BudgetRule>();
    public ICollection<BudgetPeriod> Periods { get; set; } = new List<BudgetPeriod>();
}
