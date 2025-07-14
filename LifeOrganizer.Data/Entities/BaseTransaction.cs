namespace LifeOrganizer.Data.Entities;

public class BaseTransaction : BaseEntity
{
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset OccurredOn { get; set; }
}
