using LifeOrganizer.Business.DTOs;

public class CategoryDto : BaseEntityDto
{
    public string Name { get; set; } = default!;
    public ICollection<SubcategoryDto> Subcategories { get; set; } = new List<SubcategoryDto>();
}
