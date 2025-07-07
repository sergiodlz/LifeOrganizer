namespace LifeOrganizer.Business.DTOs
{
    public class TransferDto
    {
        public decimal Amount { get; set; }
        public DateTime OccurredOn { get; set; }
        public Guid AccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public Guid SubcategoryId { get; set; }
        public int Type { get; set; }
        public int Currency { get; set; }
        public decimal? AmountTo { get; set; }
        public List<Guid> Tags { get; set; } = new();
        public Guid CategoryId { get; set; }
    }
}
