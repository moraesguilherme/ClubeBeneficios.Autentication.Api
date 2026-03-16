using ClubeBeneficios.Identity.Domain.Models.Entities;

namespace ClubeBeneficios.Identity.Domain.Interfaces.Repositories;

public interface IPartnerAccessCodeRepository
{
    Task<PartnerAccessCode?> GetByCodeAsync(string code);
    Task UpdateAsync(PartnerAccessCode code);
    Task CreateAsync(PartnerAccessCode code);
}
