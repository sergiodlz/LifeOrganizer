namespace LifeOrganizer.Data.Entities
{
    public class Pocket : BaseEntity
    {
        public required string Name { get; set; }
        public Guid AccountId { get; set; }
        public required Account Account { get; set; }
        public decimal Balance { get; set; }        
        public ICollection<PocketTransaction> Transactions { get; set; } = [];
    }
}
