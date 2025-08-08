namespace LifeOrganizer.Data.Entities;

public class Transaction : BaseTransaction
{
    public Guid AccountId { get; set; }
    public Account? Account { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public Guid? SubcategoryId { get; set; }
    public Subcategory? Subcategory { get; set; }
    public string? Description { get; set; }
    public string? ReferenceNumber { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public CurrencyType Currency { get; set; } = CurrencyType.USD;
    public decimal? ExchangeAmount { get; set; }
    public ICollection<BudgetPeriodTransaction> BudgetPeriodTransactions { get; set; } = new List<BudgetPeriodTransaction>();
}
