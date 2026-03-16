using ClubeBeneficios.Identity.Domain.Models.Entities;

namespace ClubeBeneficios.Identity.Domain.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, string role, Guid? partnerId, string accountOrigin, string jti, Guid sessionId);
    string GenerateAccessTokenForPartnerCustomer(PartnerCustomer customer, Guid partnerId, string role, string accountOrigin, string jti, Guid sessionId);
}
