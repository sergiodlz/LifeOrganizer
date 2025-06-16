using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeOrganizer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PocketsController : BaseController<Pocket, PocketDto>
{
    public PocketsController(
            IGenericService<Pocket, PocketDto> genericService)
            : base(genericService)
    {
    }
}
