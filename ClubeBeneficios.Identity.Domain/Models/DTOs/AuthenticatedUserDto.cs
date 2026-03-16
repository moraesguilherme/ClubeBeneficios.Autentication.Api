namespace ClubeBeneficios.Identity.Domain.Models.DTOs;

public class AuthenticatedUserDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? PartnerId { get; set; }
    public string AccountOrigin { get; set; } = string.Empty;
    public string? SessionId { get; set; }
}
