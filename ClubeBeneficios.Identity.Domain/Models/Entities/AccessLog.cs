namespace ClubeBeneficios.Identity.Domain.Models.Entities;

public class AccessLog
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? PartnerCustomerId { get; set; }
    public Guid? PartnerId { get; set; }
    public Guid? SessionId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Resource { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool Success { get; set; }
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; }
}
