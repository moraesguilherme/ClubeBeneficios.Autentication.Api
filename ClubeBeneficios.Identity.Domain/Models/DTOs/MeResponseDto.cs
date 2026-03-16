namespace ClubeBeneficios.Identity.Domain.Models.DTOs;

public class MeResponseDto
{
    public bool Authenticated { get; set; }
    public AuthenticatedUserDto? User { get; set; }
}
