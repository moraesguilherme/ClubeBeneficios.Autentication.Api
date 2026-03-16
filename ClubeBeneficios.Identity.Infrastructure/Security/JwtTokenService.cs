using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ClubeBeneficios.Identity.Domain.Interfaces.Services;
using ClubeBeneficios.Identity.Domain.Models.Entities;
using ClubeBeneficios.Identity.Infrastructure.Config;

namespace ClubeBeneficios.Identity.Infrastructure.Security;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public string GenerateAccessToken(User user, string role, Guid? partnerId, string accountOrigin, string jti, Guid sessionId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("session_id", sessionId.ToString()),
            new("origin", accountOrigin),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(ClaimTypes.Role, role),
            new("role", role)
        };

        if (!string.IsNullOrWhiteSpace(user.Name))
            claims.Add(new Claim(ClaimTypes.Name, user.Name));

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        if (partnerId.HasValue)
            claims.Add(new Claim("partner_id", partnerId.Value.ToString()));

        return GenerateToken(claims);
    }

    public string GenerateAccessTokenForPartnerCustomer(PartnerCustomer customer, Guid partnerId, string role, string accountOrigin, string jti, Guid sessionId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new("session_id", sessionId.ToString()),
            new("origin", accountOrigin),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(ClaimTypes.Role, role),
            new("role", role),
            new("partner_id", partnerId.ToString())
        };

        if (!string.IsNullOrWhiteSpace(customer.Name))
            claims.Add(new Claim(ClaimTypes.Name, customer.Name));

        if (!string.IsNullOrWhiteSpace(customer.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, customer.Email));
            claims.Add(new Claim(ClaimTypes.Email, customer.Email));
        }

        return GenerateToken(claims);
    }

    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
