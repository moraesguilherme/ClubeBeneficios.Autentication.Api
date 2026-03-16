using Dapper;
using ClubeBeneficios.Identity.Domain.Interfaces.Repositories;
using ClubeBeneficios.Identity.Domain.Models.Entities;
using ClubeBeneficios.Identity.Infrastructure.Persistence;

namespace ClubeBeneficios.Identity.Infrastructure.Repositories;

public class AccessLogRepository : IAccessLogRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public AccessLogRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(AccessLog log)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
INSERT INTO access_logs
(
    id, user_id, partner_customer_id, partner_id, session_id, action,
    resource, ip_address, user_agent, success, details, created_at
)
VALUES
(
    @Id, @UserId, @PartnerCustomerId, @PartnerId, @SessionId, @Action,
    @Resource, @IpAddress, @UserAgent, @Success, @Details, @CreatedAt
)";
        await connection.ExecuteAsync(sql, log);
    }
}
