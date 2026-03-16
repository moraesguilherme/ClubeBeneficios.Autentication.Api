using ClubeBeneficios.Identity.Domain.Models.Entities;

namespace ClubeBeneficios.Identity.Domain.Interfaces.Repositories;

public interface IPartnerRepository
{
    Task<Partner?> GetByIdAsync(Guid id);
    Task CreateAsync(Partner partner);
}
