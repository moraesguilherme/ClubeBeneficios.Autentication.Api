namespace ClubeBeneficios.Identity.Domain.Models.Entities;

public class Partner
{
    public Guid Id { get; set; }
    public string TradeName { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Document { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Status { get; set; }
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
