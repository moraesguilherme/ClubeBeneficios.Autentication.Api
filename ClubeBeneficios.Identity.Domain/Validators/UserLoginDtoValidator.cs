using FluentValidation;
using ClubeBeneficios.Identity.Domain.Models.DTOs;

namespace ClubeBeneficios.Identity.Domain.Validators;

public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail e obrigatorio.")
            .EmailAddress().WithMessage("Informe um e-mail valido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha e obrigatoria.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");
    }
}
