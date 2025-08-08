using LifeOrganizer.Api.Extensions;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeOrganizer.Api.Controllers
{
    [Authorize]
    public class BudgetsController : BaseController<Budget, BudgetDto>
    {
        private readonly IGenericService<Budget, BudgetDto> _genericService;
        public BudgetsController(IGenericService<Budget, BudgetDto> genericService) : base(genericService)
        {
            _genericService = genericService;
        }

        [HttpGet("{id}")]
        public override async Task<ActionResult<BudgetDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var budget = await _genericService.GetByIdWithIncludesAsync(
                id, userId, cancellationToken, a => a.Rules, a => a.Periods);
            if (budget == null)
                return NotFound();
            return Ok(budget);
        }

        [HttpGet]
        public override async Task<ActionResult<IEnumerable<BudgetDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var budgets = await _genericService.GetAllWithIncludesAsync(userId, cancellationToken, c => c.Rules, c => c.Periods);
            return Ok(budgets);
        }
    }
}
