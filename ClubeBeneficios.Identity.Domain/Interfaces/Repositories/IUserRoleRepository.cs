using ClubeBeneficios.Identity.Domain.Models.Entities;

namespace ClubeBeneficios.Identity.Domain.Interfaces.Repositories;

public interface IUserRoleRepository
{
    Task<IReadOnlyList<Role>> GetRolesByUserIdAsync(Guid userId);
    Task AssignRoleToUserAsync(Guid userId, Guid roleId);
    Task<Role?> GetByNameAsync(string name);
}
