using Dapper;
using ClubeBeneficios.Identity.Domain.Interfaces.Repositories;
using ClubeBeneficios.Identity.Domain.Models.Entities;
using ClubeBeneficios.Identity.Infrastructure.Persistence;

namespace ClubeBeneficios.Identity.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public UserRoleRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Role>> GetRolesByUserIdAsync(Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
SELECT r.*
FROM user_roles ur
INNER JOIN roles r ON r.id = ur.role_id
WHERE ur.user_id = @UserId";

        var result = await connection.QueryAsync<Role>(sql, new { UserId = userId });
        return result.ToList();
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
IF NOT EXISTS (SELECT 1 FROM user_roles WHERE user_id = @UserId AND role_id = @RoleId)
BEGIN
    INSERT INTO user_roles (id, user_id, role_id, created_at)
    VALUES (NEWID(), @UserId, @RoleId, SYSUTCDATETIME())
END";
        await connection.ExecuteAsync(sql, new { UserId = userId, RoleId = roleId });
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"SELECT TOP 1 * FROM roles WHERE name = @Name";
        return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Name = name });
    }
}
