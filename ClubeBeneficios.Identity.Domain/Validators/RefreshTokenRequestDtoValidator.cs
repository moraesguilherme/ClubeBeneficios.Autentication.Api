using FluentValidation;
using ClubeBeneficios.Identity.Domain.Models.DTOs;

namespace ClubeBeneficios.Identity.Domain.Validators;

public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("O refresh token e obrigatorio.");
    }
}
