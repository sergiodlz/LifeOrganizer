using LifeOrganizer.Business.DTOs;

public class SubcategoryDto : BaseEntityDto
{
    public string Name { get; set; } = default!;
    public Guid CategoryId { get; set; }
}
