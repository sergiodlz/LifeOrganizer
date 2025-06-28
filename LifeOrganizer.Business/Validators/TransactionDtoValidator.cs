using FluentValidation;

namespace LifeOrganizer.Business.Validators
{
    public class TransactionDtoValidator : AbstractValidator<TransactionDto>
    {
        public TransactionDtoValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.OccurredOn).NotEmpty();
            RuleFor(x => x.SubcategoryId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Currency).IsInEnum();
        }
    }
}
