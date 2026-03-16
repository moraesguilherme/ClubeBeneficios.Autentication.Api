using Dapper;
using ClubeBeneficios.Identity.Domain.Interfaces.Repositories;
using ClubeBeneficios.Identity.Domain.Models.Entities;
using ClubeBeneficios.Identity.Infrastructure.Persistence;

namespace ClubeBeneficios.Identity.Infrastructure.Repositories;

public class PartnerRepository : IPartnerRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public PartnerRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Partner?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"SELECT TOP 1 * FROM partners WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Partner>(sql, new { Id = id });
    }

    public async Task CreateAsync(Partner partner)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
INSERT INTO partners
(
    id, trade_name, legal_name, document, email, phone, status, logo_url, created_at, updated_at
)
VALUES
(
    @Id, @TradeName, @LegalName, @Document, @Email, @Phone, @Status, @LogoUrl, @CreatedAt, @UpdatedAt
)";
        await connection.ExecuteAsync(sql, partner);
    }
}
