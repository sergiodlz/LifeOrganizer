using System;

namespace LifeOrganizer.Business.DTOs;

public class BudgetRuleDto : BaseEntityDto
{
    public Guid BudgetId { get; set; }
    public BudgetDto Budget { get; set; } = null!;

    public Guid? CategoryId { get; set; }
    public CategoryDto? Category { get; set; }

    public Guid? SubcategoryId { get; set; }
    public SubcategoryDto? Subcategory { get; set; }

    public Guid? TagId { get; set; }
    public TagDto? Tag { get; set; }
}
