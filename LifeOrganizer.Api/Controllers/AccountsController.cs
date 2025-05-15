using Microsoft.AspNetCore.Authorization;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using LifeOrganizer.Api.Extensions;
using LifeOrganizer.Business.DTOs;

namespace LifeOrganizer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IGenericService<Account> _accountService;

        public AccountsController(IGenericService<Account> accountService)
        {
            _accountService = accountService;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var accounts = await _accountService.GetAllAsync(userId, cancellationToken);
            var result = accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                Name = a.Name,
                Type = a.Type,
                IncludeInGlobalBalance = a.IncludeInGlobalBalance,
                Currency = a.Currency
            });
            return Ok(result);
        }

        // GET: api/Accounts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var account = await _accountService.GetByIdAsync(id, userId, cancellationToken);
            if (account == null)
                return NotFound();

            var result = new AccountDto
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type,
                IncludeInGlobalBalance = account.IncludeInGlobalBalance,
                Currency = account.Currency
            };
            return Ok(result);
        }

        // POST: api/Accounts
        [HttpPost]
        public async Task<ActionResult<AccountDto>> Create(AccountDto dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var entity = new Account
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Type = dto.Type,
                IncludeInGlobalBalance = dto.IncludeInGlobalBalance,
                Currency = dto.Currency,
                UserId = userId
            };
            await _accountService.AddAsync(entity, cancellationToken);
            var result = new AccountDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Type = entity.Type,
                IncludeInGlobalBalance = entity.IncludeInGlobalBalance,
                Currency = entity.Currency
            };
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
        }

        // PUT: api/Accounts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, AccountDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id)
                return BadRequest("ID in URL and body do not match.");
            var userId = User.GetUserId();
            var existing = await _accountService.GetByIdAsync(id, userId, cancellationToken);
            if (existing == null)
                return NotFound();
            existing.Name = dto.Name;
            existing.Type = dto.Type;
            existing.IncludeInGlobalBalance = dto.IncludeInGlobalBalance;
            existing.Currency = dto.Currency;
            await _accountService.UpdateAsync(existing, cancellationToken);
            return NoContent();
        }

        // DELETE: api/Accounts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var account = await _accountService.GetByIdAsync(id, userId, cancellationToken);
            if (account == null)
                return NotFound();
            // Soft delete: mark as deleted
            await _accountService.RemoveAsync(account, cancellationToken);
            return NoContent();
        }
    }
}
