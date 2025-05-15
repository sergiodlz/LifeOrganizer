using Microsoft.AspNetCore.Authorization;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Api.Controllers
{
    [Authorize]
    public class SubcategoriesController : BaseController<Subcategory, SubcategoryDto>
    {
        public SubcategoriesController(IGenericService<Subcategory, SubcategoryDto> genericService)
            : base(genericService)
        {
        }
    }
}
