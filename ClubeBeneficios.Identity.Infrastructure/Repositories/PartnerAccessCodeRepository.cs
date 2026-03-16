using Dapper;
using ClubeBeneficios.Identity.Domain.Interfaces.Repositories;
using ClubeBeneficios.Identity.Domain.Models.Entities;
using ClubeBeneficios.Identity.Infrastructure.Persistence;

namespace ClubeBeneficios.Identity.Infrastructure.Repositories;

public class PartnerAccessCodeRepository : IPartnerAccessCodeRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public PartnerAccessCodeRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PartnerAccessCode?> GetByCodeAsync(string code)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                            SELECT TOP 1
                                id AS Id,
                                partner_id AS PartnerId,
                                created_by_user_id AS CreatedByUserId,
                                code AS Code,
                                description AS Description,
                                status AS Status,
                                expires_at AS ExpiresAt,
                                max_uses AS MaxUses,
                                used_count AS UsedCount,
                                created_at AS CreatedAt,
                                updated_at AS UpdatedAt
                            FROM partner_access_codes
                            WHERE code = @Code";

        return await connection.QueryFirstOrDefaultAsync<PartnerAccessCode>(sql, new { Code = code });
    }

    public async Task UpdateAsync(PartnerAccessCode code)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
                            UPDATE partner_access_codes
                            SET
                                partner_id = @PartnerId,
                                created_by_user_id = @CreatedByUserId,
                                code = @Code,
                                description = @Description,
                                status = @Status,
                                expires_at = @ExpiresAt,
                                max_uses = @MaxUses,
                                used_count = @UsedCount,
                                updated_at = @UpdatedAt
                            WHERE id = @Id";
        await connection.ExecuteAsync(sql, code);
    }

    public async Task CreateAsync(PartnerAccessCode code)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
                            INSERT INTO partner_access_codes
                            (
                                id, partner_id, created_by_user_id, code, description, status,
                                expires_at, max_uses, used_count, created_at, updated_at
                            )
                            VALUES
                            (
                                @Id, @PartnerId, @CreatedByUserId, @Code, @Description, @Status,
                                @ExpiresAt, @MaxUses, @UsedCount, @CreatedAt, @UpdatedAt
                            )";
        await connection.ExecuteAsync(sql, code);
    }
}
