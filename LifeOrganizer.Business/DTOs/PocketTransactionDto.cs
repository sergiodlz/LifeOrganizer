using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.DTOs;

public class PocketTransactionDto : BaseEntityDto
{
    public Guid PocketId { get; set; }
    public string? PocketName { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset OccurredOn { get; set; }
}
