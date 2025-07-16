using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace LifeOrganizer.Api.Controllers
{
    [Authorize]
    public class PocketTransactionsController : BaseController<PocketTransaction, PocketTransactionDto>
    {
        private readonly IPocketTransactionService _pocketTransactionService;
        public PocketTransactionsController(
            IPocketTransactionService pocketTransactionService)
            : base(pocketTransactionService)
        {
            _pocketTransactionService = pocketTransactionService;
        }
    }
}
