namespace LifeOrganizer.Data.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
