
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.Services;

public interface ICategoryService : IGenericService<Category, CategoryDto>
{
    Task<CategoryDto> AddWithDefaultSubcategoryAsync(CategoryDto dto, CancellationToken cancellationToken);
}
