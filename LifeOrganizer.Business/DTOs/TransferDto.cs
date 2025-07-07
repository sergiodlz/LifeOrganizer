namespace LifeOrganizer.Business.DTOs
{
    public class TransferDto
    {
        public decimal Amount { get; set; }
        public DateTime OccurredOn { get; set; }
        public Guid AccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public Guid? SubcategoryId { get; set; }
        public decimal? AmountTo { get; set; }
        public List<TagDto> Tags { get; set; } = new();
        public Guid? CategoryId { get; set; }
    }
}
