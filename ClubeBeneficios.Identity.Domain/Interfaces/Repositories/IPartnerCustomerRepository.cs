using ClubeBeneficios.Identity.Domain.Models.Entities;

namespace ClubeBeneficios.Identity.Domain.Interfaces.Repositories;

public interface IPartnerCustomerRepository
{
    Task<PartnerCustomer?> GetByIdAsync(Guid id);
    Task CreateAsync(PartnerCustomer customer);
    Task UpdateAsync(PartnerCustomer customer);
}
