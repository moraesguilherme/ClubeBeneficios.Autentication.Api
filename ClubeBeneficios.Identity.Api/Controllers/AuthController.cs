using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClubeBeneficios.Identity.Domain.Interfaces.Services;
using ClubeBeneficios.Identity.Domain.Models.DTOs;

namespace ClubeBeneficios.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("partner-code")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWithPartnerCode([FromBody] PartnerCodeLoginDto dto)
    {
        var result = await _authService.LoginWithPartnerCodeAsync(dto);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto);
        return Ok(result);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto)
    {
        await _authService.LogoutAsync(dto);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? User.FindFirstValue("sub");

        var role = User.FindFirstValue(ClaimTypes.Role)
                   ?? User.FindFirstValue("role");

        var partnerId = User.FindFirstValue("partner_id");

        var email = User.FindFirstValue(ClaimTypes.Email)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Email)
                    ?? User.FindFirstValue("email");

        var sessionId = User.FindFirstValue("session_id");
        var origin = User.FindFirstValue("origin");

        var result = await _authService.GetMeAsync(userId, role, partnerId, email, sessionId, origin);
        return Ok(result);
    }

    [HttpGet("test-secure")]
    [Authorize]
    public IActionResult TestSecure()
    {
        var response = new SecureTestResponseDto
        {
            Authenticated = User.Identity?.IsAuthenticated ?? false,
            Message = "Acesso autorizado com sucesso.",
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? User.FindFirstValue("sub"),
            Email = User.FindFirstValue(ClaimTypes.Email)
                    ?? User.FindFirstValue(JwtRegisteredClaimNames.Email)
                    ?? User.FindFirstValue("email"),
            Role = User.FindFirstValue(ClaimTypes.Role)
                   ?? User.FindFirstValue("role"),
            PartnerId = User.FindFirstValue("partner_id"),
            SessionId = User.FindFirstValue("session_id"),
            Origin = User.FindFirstValue("origin"),
            UtcNow = DateTime.UtcNow
        };

        return Ok(response);
    }
}
