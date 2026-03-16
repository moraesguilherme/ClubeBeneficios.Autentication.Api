using Dapper;
using ClubeBeneficios.Identity.Domain.Interfaces.Repositories;
using ClubeBeneficios.Identity.Domain.Models.Entities;
using ClubeBeneficios.Identity.Infrastructure.Persistence;

namespace ClubeBeneficios.Identity.Infrastructure.Repositories;

public class PartnerCustomerRepository : IPartnerCustomerRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public PartnerCustomerRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PartnerCustomer?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"SELECT TOP 1 * FROM partner_customers WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<PartnerCustomer>(sql, new { Id = id });
    }

    public async Task CreateAsync(PartnerCustomer customer)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
                            INSERT INTO partner_customers
                            (
                                id, partner_id, origin_code_id, name, email, phone, status,
                                created_at, last_access_at, updated_at
                            )
                            VALUES
                            (
                                @Id, @PartnerId, @OriginCodeId, @Name, @Email, @Phone, @Status,
                                @CreatedAt, @LastAccessAt, @UpdatedAt
                            )";
        await connection.ExecuteAsync(sql, customer);
    }

    public async Task UpdateAsync(PartnerCustomer customer)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
                            UPDATE partner_customers
                            SET
                                partner_id = @PartnerId,
                                origin_code_id = @OriginCodeId,
                                name = @Name,
                                email = @Email,
                                phone = @Phone,
                                status = @Status,
                                last_access_at = @LastAccessAt,
                                updated_at = @UpdatedAt
                            WHERE id = @Id";
        await connection.ExecuteAsync(sql, customer);
    }
}
