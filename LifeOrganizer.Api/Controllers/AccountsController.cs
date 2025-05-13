using Microsoft.AspNetCore.Authorization;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using LifeOrganizer.Api.Extensions;

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
        public async Task<ActionResult<IEnumerable<Account>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var accounts = await _accountService.GetAllAsync(userId, cancellationToken);
            return Ok(accounts);
        }

        // GET: api/Accounts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var account = await _accountService.GetByIdAsync(id, userId, cancellationToken);
            if (account == null)
                return NotFound();
            return Ok(account);
        }

        // POST: api/Accounts
        [HttpPost]
        public async Task<ActionResult<Account>> Create(Account account, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            account.UserId = userId;
            await _accountService.AddAsync(account, cancellationToken);
            // Return 201 Created with location header
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        // PUT: api/Accounts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Account account, CancellationToken cancellationToken)
        {
            if (id != account.Id)
                return BadRequest("ID in URL and body do not match.");
            var userId = User.GetUserId();
            var existing = await _accountService.GetByIdAsync(id, userId, cancellationToken);
            if (existing == null)
                return NotFound();
            account.UserId = userId;
            await _accountService.UpdateAsync(account, cancellationToken);
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
