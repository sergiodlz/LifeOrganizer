using FluentValidation;

namespace LifeOrganizer.Business.Validators
{
    public class TransactionDtoValidator : AbstractValidator<TransactionDto>
    {
        public TransactionDtoValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.Description).NotEmpty().MaximumLength(255);
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();
        }
    }
}
