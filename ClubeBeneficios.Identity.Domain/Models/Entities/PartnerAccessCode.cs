namespace ClubeBeneficios.Identity.Domain.Models.Entities;

public class PartnerAccessCode
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public string Code { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int? MaxUses { get; set; }
    public int UsedCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
