using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;

public class TransactionDto : BaseEntityDto
{
    public decimal Amount { get; set; }
    public DateTimeOffset OccurredOn { get; set; }
    public Guid AccountId { get; set; }
    public Guid? SubcategoryId { get; set; }
    public TransactionType Type { get; set; }
    public CurrencyType Currency { get; set; }
    public AccountDto? Account { get; set; }
    public CategoryDto? Category { get; set; }
    public SubcategoryDto? Subcategory { get; set; }
    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
}
