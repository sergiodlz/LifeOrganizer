using LifeOrganizer.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using LifeOrganizer.Business.DTOs;

namespace LifeOrganizer.Api.Controllers
{
    [Authorize]
    public class TransactionsController : BaseController<Transaction, TransactionDto>
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
            : base(transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public override async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var transactions = await _transactionService.GetAllWithIncludesAsync(userId, cancellationToken, t => t.Tags, t => t.Category, t => t.Subcategory, t => t.Account);
            return Ok(transactions);
        }
    }
}
