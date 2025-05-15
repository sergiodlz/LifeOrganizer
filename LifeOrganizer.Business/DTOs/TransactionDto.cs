using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;

public class TransactionDto : BaseEntityDto
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = default!;
    public Guid AccountId { get; set; }
    public TransactionType Type { get; set; }
}
