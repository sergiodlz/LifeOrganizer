namespace LifeOrganizer.Data.Entities;

public class Tag : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
