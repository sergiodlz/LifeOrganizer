using Microsoft.AspNetCore.Authorization;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Business.DTOs;

namespace LifeOrganizer.Api.Controllers
{
    [Authorize]
    public class AccountsController : BaseController<Account, AccountDto>
    {
        public AccountsController(IGenericService<Account, AccountDto> genericService)
            : base(genericService)
        {
        }
    }
}
