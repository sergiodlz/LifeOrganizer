using FluentValidation;
using LifeOrganizer.Business.DTOs.Auth;

namespace LifeOrganizer.Business.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<RegisterDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Username).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        }
    }
}
