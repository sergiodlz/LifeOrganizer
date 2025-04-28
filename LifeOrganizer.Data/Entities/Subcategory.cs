namespace LifeOrganizer.Data.Entities;

public class Subcategory : BaseEntity
{
    public required string Name { get; set; }
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}
