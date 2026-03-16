namespace ClubeBeneficios.Identity.Domain.Models.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid? PartnerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? Phone { get; set; }
    public string Status { get; set; }
    public string UserType { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
