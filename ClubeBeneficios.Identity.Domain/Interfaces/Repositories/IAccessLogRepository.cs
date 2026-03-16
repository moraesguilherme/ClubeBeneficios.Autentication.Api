using ClubeBeneficios.Identity.Domain.Models.Entities;

namespace ClubeBeneficios.Identity.Domain.Interfaces.Repositories;

public interface IAccessLogRepository
{
    Task CreateAsync(AccessLog log);
}
