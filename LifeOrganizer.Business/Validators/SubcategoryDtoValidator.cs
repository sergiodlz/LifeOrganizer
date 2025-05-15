using FluentValidation;

namespace LifeOrganizer.Business.Validators
{
    public class SubcategoryDtoValidator : AbstractValidator<SubcategoryDto>
    {
        public SubcategoryDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}
