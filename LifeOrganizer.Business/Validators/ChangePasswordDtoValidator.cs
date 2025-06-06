using FluentValidation;
using LifeOrganizer.Business.DTOs.Auth;

namespace LifeOrganizer.Business.Validators
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().MinimumLength(6).MaximumLength(100);
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).MaximumLength(100);
        }
    }
}
