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
        public TransactionsController(IGenericService<Transaction, TransactionDto> genericService)
            : base(genericService)
        {
        }
    }
}
