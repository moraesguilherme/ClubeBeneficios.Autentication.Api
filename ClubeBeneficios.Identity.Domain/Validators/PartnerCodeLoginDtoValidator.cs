using FluentValidation;
using ClubeBeneficios.Identity.Domain.Models.DTOs;

namespace ClubeBeneficios.Identity.Domain.Validators;

public class PartnerCodeLoginDtoValidator : AbstractValidator<PartnerCodeLoginDto>
{
    public PartnerCodeLoginDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("O codigo do parceiro e obrigatorio.")
            .MinimumLength(3).WithMessage("O codigo informado e invalido.");
    }
}
