namespace LifeOrganizer.Data.Entities;

public class Account : BaseEntity
{
    public required string Name { get; set; }
    public AccountType Type { get; set; }
    public bool IncludeInGlobalBalance { get; set; }
}
