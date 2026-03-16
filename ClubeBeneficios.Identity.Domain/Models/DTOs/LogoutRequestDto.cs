namespace ClubeBeneficios.Identity.Domain.Models.DTOs;

public class LogoutRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
