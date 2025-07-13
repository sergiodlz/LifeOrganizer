namespace LifeOrganizer.Data.Entities;

public class Account : BaseEntity
{
    public required string Name { get; set; }
    public AccountType Type { get; set; }
    public bool IncludeInGlobalBalance { get; set; }
    public CurrencyType Currency { get; set; } = CurrencyType.USD;
    public decimal Balance { get; set; }
    public ICollection<Pocket> Pockets { get; set; } = [];
    public ICollection<Transaction> Transactions { get; set; } = [];
}
