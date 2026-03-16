namespace ClubeBeneficios.Identity.Domain.Models.Entities;

public class Session
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? PartnerCustomerId { get; set; }
    public string AccessTokenJti { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsValid { get; set; }
}
