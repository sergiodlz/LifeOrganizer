using FluentValidation;
using LifeOrganizer.Business.DTOs;

namespace LifeOrganizer.Business.Validators
{
    public class AccountDtoValidator : AbstractValidator<AccountDto>
    {
        public AccountDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Currency).IsInEnum();
        }
    }
}
