namespace ClubeBeneficios.Identity.Domain.Interfaces.Http;

public interface IRequestContextAccessor
{
    string? GetIpAddress();
    string? GetUserAgent();
}
