namespace ClubeBeneficios.Identity.Domain.Models.Entities;

public class PartnerCustomer
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }
    public Guid? OriginCodeId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastAccessAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
