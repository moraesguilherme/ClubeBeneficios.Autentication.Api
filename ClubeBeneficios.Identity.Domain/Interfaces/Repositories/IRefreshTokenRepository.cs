using ClubeBeneficios.Identity.Domain.Models.Entities;

namespace ClubeBeneficios.Identity.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task UpdateAsync(RefreshToken refreshToken);
}
