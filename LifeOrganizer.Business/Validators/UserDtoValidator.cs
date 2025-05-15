using FluentValidation;
using LifeOrganizer.Business.DTOs.Auth;

namespace LifeOrganizer.Business.Validators
{
    public class UserDtoValidator : AbstractValidator<LoginDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.UsernameOrEmail).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        }
    }
}
