using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ClubeBeneficios.Identity.Domain.Interfaces.Http;
using ClubeBeneficios.Identity.Domain.Interfaces.Repositories;
using ClubeBeneficios.Identity.Domain.Interfaces.Services;
using ClubeBeneficios.Identity.Domain.Models.DTOs;
using ClubeBeneficios.Identity.Domain.Services;
using ClubeBeneficios.Identity.Domain.Validators;
using ClubeBeneficios.Identity.Infrastructure.Config;
using ClubeBeneficios.Identity.Infrastructure.Http;
using ClubeBeneficios.Identity.Infrastructure.Persistence;
using ClubeBeneficios.Identity.Infrastructure.Repositories;
using ClubeBeneficios.Identity.Infrastructure.Security;

namespace ClubeBeneficios.Identity.Infrastructure.Extensions;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<IRequestContextAccessor, RequestContextAccessor>();

        services.AddScoped<SqlConnectionFactory>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPartnerAccessCodeRepository, PartnerAccessCodeRepository>();
        services.AddScoped<IPartnerCustomerRepository, PartnerCustomerRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAccessLogRepository, AccessLogRepository>();
        services.AddScoped<IPartnerRepository, PartnerRepository>();

        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IValidator<UserLoginDto>, UserLoginDtoValidator>();
        services.AddScoped<IValidator<PartnerCodeLoginDto>, PartnerCodeLoginDtoValidator>();
        services.AddScoped<IValidator<RefreshTokenRequestDto>, RefreshTokenRequestDtoValidator>();
        services.AddScoped<IValidator<LogoutRequestDto>, LogoutRequestDtoValidator>();

        return services;
    }
}
