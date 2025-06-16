using FluentValidation;
using LifeOrganizer.Business.DTOs;

namespace LifeOrganizer.Business.Validators;

public class PocketDtoValidator : AbstractValidator<PocketDto>
{
    public PocketDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
