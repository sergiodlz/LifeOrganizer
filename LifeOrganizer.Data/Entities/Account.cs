namespace LifeOrganizer.Data.Entities;

public class Account : BaseEntity
{
    public required string Name { get; set; }
    public AccountType Type { get; set; }
    public bool IncludeInGlobalBalance { get; set; }
    public CurrencyType Currency { get; set; } = CurrencyType.USD;
    public ICollection<Pocket> Pockets { get; set; } = [];
}
