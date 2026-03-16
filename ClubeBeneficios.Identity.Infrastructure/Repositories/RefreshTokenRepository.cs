using Dapper;
using ClubeBeneficios.Identity.Domain.Interfaces.Repositories;
using ClubeBeneficios.Identity.Domain.Models.Entities;
using ClubeBeneficios.Identity.Infrastructure.Persistence;

namespace ClubeBeneficios.Identity.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public RefreshTokenRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(RefreshToken refreshToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
INSERT INTO refresh_tokens
(
    id, session_id, user_id, partner_customer_id, token,
    expires_at, created_at, revoked_at, replaced_by_token, created_by_ip
)
VALUES
(
    @Id, @SessionId, @UserId, @PartnerCustomerId, @Token,
    @ExpiresAt, @CreatedAt, @RevokedAt, @ReplacedByToken, @CreatedByIp
)";
        await connection.ExecuteAsync(sql, refreshToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                            SELECT TOP 1
                                id AS Id,
                                session_id AS SessionId,
                                user_id AS UserId,
                                partner_customer_id AS PartnerCustomerId,
                                token AS Token,
                                created_at AS CreatedAt,
                                expires_at AS ExpiresAt,
                                revoked_at AS RevokedAt,
                                replaced_by_token AS ReplacedByToken,
                                created_by_ip AS CreatedByIp
                            FROM refresh_tokens
                            WHERE token = @Token";

        return await connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { Token = token });
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
UPDATE refresh_tokens
SET
    session_id = @SessionId,
    user_id = @UserId,
    partner_customer_id = @PartnerCustomerId,
    token = @Token,
    expires_at = @ExpiresAt,
    created_at = @CreatedAt,
    revoked_at = @RevokedAt,
    replaced_by_token = @ReplacedByToken,
    created_by_ip = @CreatedByIp
WHERE id = @Id";
        await connection.ExecuteAsync(sql, refreshToken);
    }
}
