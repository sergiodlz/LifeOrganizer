using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LifeOrganizer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var accounts = await _accountService.GetAllAsync(cancellationToken);
            return Ok(accounts);
        }

        // GET: api/Accounts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var account = await _accountService.GetByIdAsync(id, cancellationToken);
            if (account == null)
                return NotFound();
            return Ok(account);
        }

        // POST: api/Accounts
        [HttpPost]
        public async Task<ActionResult<Account>> Create(Account account, CancellationToken cancellationToken)
        {
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
            var existing = await _accountService.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                return NotFound();
            await _accountService.UpdateAsync(account, cancellationToken);
            return NoContent();
        }

        // DELETE: api/Accounts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var account = await _accountService.GetByIdAsync(id, cancellationToken);
            if (account == null)
                return NotFound();
            await _accountService.RemoveAsync(account, cancellationToken);
            return NoContent();
        }
    }
}
