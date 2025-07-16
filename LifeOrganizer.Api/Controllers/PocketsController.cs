using LifeOrganizer.Api.Extensions;
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
    private readonly IGenericService<Pocket, PocketDto> _genericService;
    public PocketsController(
            IGenericService<Pocket, PocketDto> genericService)
            : base(genericService)
    {
        _genericService = genericService;
    }

    [HttpGet("{id}")]
    public override async Task<ActionResult<PocketDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var pocket = await _genericService.GetByIdWithIncludesAsync(
            id, userId, cancellationToken, a => a.Transactions);
        if (pocket == null)
            return NotFound();
        return Ok(pocket);
    }
}
