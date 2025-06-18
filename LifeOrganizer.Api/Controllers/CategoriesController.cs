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
        private readonly IGenericService<Category, CategoryDto> _genericService;

        public CategoriesController(IGenericService<Category, CategoryDto> genericService)
            : base(genericService)
        {
            _genericService = genericService;
        }

        [HttpGet]
        public override async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var categories = await _genericService.GetAllWithIncludesAsync(userId, cancellationToken, c => c.Subcategories);
            return Ok(categories);
        }
    }
}
