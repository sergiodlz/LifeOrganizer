namespace LifeOrganizer.Data.Entities;

public class BudgetRule : BaseEntity
{
    public Guid BudgetId { get; set; }
    public Budget Budget { get; set; } = null!;

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid? SubcategoryId { get; set; }
    public Subcategory? Subcategory { get; set; }

    public Guid? TagId { get; set; }
    public Tag? Tag { get; set; }
}
