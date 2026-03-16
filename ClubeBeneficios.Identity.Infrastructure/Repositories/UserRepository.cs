using Dapper;
using ClubeBeneficios.Identity.Domain.Interfaces.Repositories;
using ClubeBeneficios.Identity.Domain.Models.Entities;
using ClubeBeneficios.Identity.Infrastructure.Persistence;

namespace ClubeBeneficios.Identity.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public UserRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
        SELECT TOP 1
            id AS Id,
            partner_id AS PartnerId,
            name AS Name,
            email AS Email,
            password_hash AS PasswordHash,
            phone AS Phone,
            status AS Status,
            user_type AS UserType,
            email_confirmed AS EmailConfirmed,
            last_login_at AS LastLoginAt,
            created_at AS CreatedAt,
            updated_at AS UpdatedAt
        FROM users
        WHERE email = @Email";

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"SELECT TOP 1 * FROM users WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"SELECT COUNT(1) FROM users WHERE email = @Email";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });
        return count > 0;
    }

    public async Task CreateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
INSERT INTO users
(
    id, partner_id, name, email, password_hash, phone, status, user_type,
    email_confirmed, last_login_at, created_at, updated_at
)
VALUES
(
    @Id, @PartnerId, @Name, @Email, @PasswordHash, @Phone, @Status, @UserType,
    @EmailConfirmed, @LastLoginAt, @CreatedAt, @UpdatedAt
)";
        await connection.ExecuteAsync(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
UPDATE users
SET
    partner_id = @PartnerId,
    name = @Name,
    email = @Email,
    password_hash = @PasswordHash,
    phone = @Phone,
    status = @Status,
    user_type = @UserType,
    email_confirmed = @EmailConfirmed,
    last_login_at = @LastLoginAt,
    updated_at = @UpdatedAt
WHERE id = @Id";
        await connection.ExecuteAsync(sql, user);
    }
}
