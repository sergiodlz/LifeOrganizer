using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Api.Extensions;

namespace LifeOrganizer.Api.Controllers
{
    [Authorize]
    public class AccountsController : BaseController<Account, AccountDto>
    {
        private readonly IGenericService<Pocket, PocketDto> _pocketService;
        private readonly IGenericService<Account, AccountDto> _genericService;

        public AccountsController(
            IGenericService<Account, AccountDto> genericService,
            IGenericService<Pocket, PocketDto> pocketService)
            : base(genericService)
        {
            _pocketService = pocketService;
            _genericService = genericService;
        }

        [HttpPost("{accountId}/pockets")]
        public async Task<IActionResult> CreatePocket(Guid accountId, [FromBody] PocketDto dto, CancellationToken cancellationToken = default)
        {
            dto.AccountId = accountId;
            dto.UserId = User.GetUserId();
            await _pocketService.AddAsync(dto, cancellationToken);
            return Created(string.Empty, dto);
        }

        [HttpGet("{accountId}/pockets")]
        public async Task<ActionResult<IEnumerable<PocketDto>>> GetPockets(Guid accountId, CancellationToken cancellationToken = default)
        {
            var userId = User.GetUserId();
            var pockets = await _pocketService.FindAsync(p => p.AccountId == accountId, userId, cancellationToken);
            return Ok(pockets);
        }

        [HttpGet]
        public override async Task<ActionResult<IEnumerable<AccountDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var accounts = await _genericService.GetAllWithIncludesAsync(userId, cancellationToken, a => a.Pockets);
            return Ok(accounts);
        }
    }
}
