namespace ClubeBeneficios.Identity.Domain.Interfaces.Services;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();
}
