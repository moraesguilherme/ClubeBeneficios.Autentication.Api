namespace ClubeBeneficios.Identity.Domain.Models.DTOs;

public class AuthenticateResultDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public AuthenticatedUserDto? User { get; set; }
}
