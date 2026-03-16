using ClubeBeneficios.Identity.Domain.Models.DTOs;

namespace ClubeBeneficios.Identity.Domain.Interfaces.Services;

public interface IAuthService
{
    Task<AuthenticateResultDto> LoginAsync(UserLoginDto dto);
    Task<AuthenticateResultDto> LoginWithPartnerCodeAsync(PartnerCodeLoginDto dto);
    Task<AuthenticateResultDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
    Task LogoutAsync(LogoutRequestDto dto);
    Task<MeResponseDto> GetMeAsync(string? userId, string? role, string? partnerId, string? email, string? sessionId, string? origin);
}
