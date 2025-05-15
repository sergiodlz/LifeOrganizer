using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.DTOs;

public class AccountDto : BaseEntityDto
{
    public string Name { get; set; } = default!;
    public AccountType Type { get; set; }
    public bool IncludeInGlobalBalance { get; set; }
    public CurrencyType Currency { get; set; }
}
