using Microsoft.AspNetCore.Http;
using ClubeBeneficios.Identity.Domain.Interfaces.Http;

namespace ClubeBeneficios.Identity.Infrastructure.Http;

public class RequestContextAccessor : IRequestContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;
        return context?.Connection?.RemoteIpAddress?.ToString();
    }

    public string? GetUserAgent()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
            return null;

        if (context.Request.Headers.TryGetValue("User-Agent", out var userAgent))
            return userAgent.ToString();

        return null;
    }
}
