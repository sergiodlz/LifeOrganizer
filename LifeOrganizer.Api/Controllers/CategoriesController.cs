using Microsoft.AspNetCore.Authorization;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using LifeOrganizer.Api.Extensions;

namespace LifeOrganizer.Api.Controllers
{
    [Authorize]
    public class CategoriesController : BaseController<Category, CategoryDto>
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
            : base(categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public override async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var categories = await _categoryService.GetAllWithIncludesAsync(userId, cancellationToken, c => c.Subcategories);
            return Ok(categories);
        }

        [HttpPost]
        public override async Task<ActionResult<CategoryDto>> Create(CategoryDto dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            dto.UserId = userId;
            await _categoryService.AddWithDefaultSubcategoryAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
    }
}
