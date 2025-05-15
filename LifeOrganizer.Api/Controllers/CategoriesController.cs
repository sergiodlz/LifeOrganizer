using Microsoft.AspNetCore.Authorization;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Api.Controllers
{
    [Authorize]
    public class CategoriesController : BaseController<Category, CategoryDto>
    {
        public CategoriesController(IGenericService<Category, CategoryDto> genericService)
            : base(genericService)
        {
        }
    }
}
