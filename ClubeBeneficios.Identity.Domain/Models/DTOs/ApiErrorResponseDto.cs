namespace ClubeBeneficios.Identity.Domain.Models.DTOs;

public class ApiErrorResponseDto
{
    public string Message { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public int StatusCode { get; set; }
}
