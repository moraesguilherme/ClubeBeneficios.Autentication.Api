using ClubeBeneficios.Identity.Domain.Models.Entities;

namespace ClubeBeneficios.Identity.Domain.Interfaces.Repositories;

public interface ISessionRepository
{
    Task CreateAsync(Session session);
    Task<Session?> GetByIdAsync(Guid id);
    Task<Session?> GetByAccessTokenJtiAsync(string jti);
    Task UpdateAsync(Session session);
    Task RevokeByIdAsync(Guid id, DateTime revokedAtUtc);
}
