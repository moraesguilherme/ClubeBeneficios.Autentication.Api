namespace ClubeBeneficios.Identity.Domain.Models.DTOs;

public class SecureTestResponseDto
{
    public bool Authenticated { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? PartnerId { get; set; }
    public string? SessionId { get; set; }
    public string? Origin { get; set; }
    public DateTime UtcNow { get; set; }
}
