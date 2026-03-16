using Dapper;
using ClubeBeneficios.Identity.Domain.Interfaces.Repositories;
using ClubeBeneficios.Identity.Domain.Models.Entities;
using ClubeBeneficios.Identity.Infrastructure.Persistence;

namespace ClubeBeneficios.Identity.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public SessionRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(Session session)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
                            INSERT INTO sessions
                            (
                                id, user_id, partner_customer_id, access_token_jti, ip_address, user_agent,
                                created_at, expires_at, revoked_at, is_valid
                            )
                            VALUES
                            (
                                @Id, @UserId, @PartnerCustomerId, @AccessTokenJti, @IpAddress, @UserAgent,
                                @CreatedAt, @ExpiresAt, @RevokedAt, @IsValid
                            )";
        await connection.ExecuteAsync(sql, session);
    }

    public async Task<Session?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                            SELECT TOP 1
                                id AS Id,
                                user_id AS UserId,
                                partner_customer_id AS PartnerCustomerId,
                                access_token_jti AS AccessTokenJti,
                                ip_address AS IpAddress,
                                user_agent AS UserAgent,
                                created_at AS CreatedAt,
                                expires_at AS ExpiresAt,
                                revoked_at AS RevokedAt,
                                is_valid AS IsValid
                            FROM sessions
                            WHERE id = @Id";

        return await connection.QueryFirstOrDefaultAsync<Session>(sql, new { Id = id });
    }

    public async Task<Session?> GetByAccessTokenJtiAsync(string jti)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                            SELECT TOP 1
                                id AS Id,
                                user_id AS UserId,
                                partner_customer_id AS PartnerCustomerId,
                                access_token_jti AS AccessTokenJti,
                                ip_address AS IpAddress,
                                user_agent AS UserAgent,
                                created_at AS CreatedAt,
                                expires_at AS ExpiresAt,
                                revoked_at AS RevokedAt,
                                is_valid AS IsValid
                            FROM sessions
                            WHERE access_token_jti = @Jti";

        return await connection.QueryFirstOrDefaultAsync<Session>(sql, new { Jti = jti });
    }

    public async Task UpdateAsync(Session session)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
                            UPDATE sessions
                            SET
                                user_id = @UserId,
                                partner_customer_id = @PartnerCustomerId,
                                access_token_jti = @AccessTokenJti,
                                ip_address = @IpAddress,
                                user_agent = @UserAgent,
                                created_at = @CreatedAt,
                                expires_at = @ExpiresAt,
                                revoked_at = @RevokedAt,
                                is_valid = @IsValid
                            WHERE id = @Id";
        await connection.ExecuteAsync(sql, session);
    }

    public async Task RevokeByIdAsync(Guid id, DateTime revokedAtUtc)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
                            UPDATE sessions
                            SET revoked_at = @RevokedAt, is_valid = 0
                            WHERE id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id, RevokedAt = revokedAtUtc });
    }
}
