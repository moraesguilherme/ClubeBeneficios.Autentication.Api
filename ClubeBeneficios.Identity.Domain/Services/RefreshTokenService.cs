using System.Security.Cryptography;
using ClubeBeneficios.Identity.Domain.Interfaces.Services;

namespace ClubeBeneficios.Identity.Domain.Services;

public class RefreshTokenService : IRefreshTokenService
{
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
