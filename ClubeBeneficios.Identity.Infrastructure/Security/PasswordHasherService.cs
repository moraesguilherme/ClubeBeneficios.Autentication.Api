using Microsoft.AspNetCore.Identity;
using ClubeBeneficios.Identity.Domain.Interfaces.Services;
using ClubeBeneficios.Identity.Domain.Models.Entities;

namespace ClubeBeneficios.Identity.Infrastructure.Security;

public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(string password)
    {
        var user = new User();
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var user = new User();
        var result = _passwordHasher.VerifyHashedPassword(user, passwordHash, password);

        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}