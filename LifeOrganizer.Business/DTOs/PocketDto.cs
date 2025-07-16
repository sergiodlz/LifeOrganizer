namespace LifeOrganizer.Business.DTOs;

public class PocketDto : BaseEntityDto
{
    public string Name { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public decimal Balance { get; set; }
    public ICollection<PocketTransactionDto> Transactions { get; set; } = [];
}
