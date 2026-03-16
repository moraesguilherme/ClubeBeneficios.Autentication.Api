using FluentValidation;
using ClubeBeneficios.Identity.Domain.Interfaces.Http;
using ClubeBeneficios.Identity.Domain.Interfaces.Repositories;
using ClubeBeneficios.Identity.Domain.Interfaces.Services;
using ClubeBeneficios.Identity.Domain.Models.DTOs;
using ClubeBeneficios.Identity.Domain.Models.Entities;

namespace ClubeBeneficios.Identity.Domain.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPartnerAccessCodeRepository _partnerAccessCodeRepository;
    private readonly IPartnerCustomerRepository _partnerCustomerRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRequestContextAccessor _requestContextAccessor;
    private readonly IValidator<UserLoginDto> _userLoginValidator;
    private readonly IValidator<PartnerCodeLoginDto> _partnerCodeValidator;
    private readonly IValidator<RefreshTokenRequestDto> _refreshTokenValidator;
    private readonly IValidator<LogoutRequestDto> _logoutValidator;

    public AuthService(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IPartnerAccessCodeRepository partnerAccessCodeRepository,
        IPartnerCustomerRepository partnerCustomerRepository,
        ISessionRepository sessionRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAccessLogRepository accessLogRepository,
        IPasswordHasherService passwordHasherService,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IRequestContextAccessor requestContextAccessor,
        IValidator<UserLoginDto> userLoginValidator,
        IValidator<PartnerCodeLoginDto> partnerCodeValidator,
        IValidator<RefreshTokenRequestDto> refreshTokenValidator,
        IValidator<LogoutRequestDto> logoutValidator)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _partnerAccessCodeRepository = partnerAccessCodeRepository;
        _partnerCustomerRepository = partnerCustomerRepository;
        _sessionRepository = sessionRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _accessLogRepository = accessLogRepository;
        _passwordHasherService = passwordHasherService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _requestContextAccessor = requestContextAccessor;
        _userLoginValidator = userLoginValidator;
        _partnerCodeValidator = partnerCodeValidator;
        _refreshTokenValidator = refreshTokenValidator;
        _logoutValidator = logoutValidator;
    }

    public async Task<AuthenticateResultDto> LoginAsync(UserLoginDto dto)
    {
        await _userLoginValidator.ValidateAndThrowAsync(dto);

        var ip = _requestContextAccessor.GetIpAddress();
        var userAgent = _requestContextAccessor.GetUserAgent();

        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user is null || !_passwordHasherService.VerifyPassword(dto.Password, user.PasswordHash))
        {
            await _accessLogRepository.CreateAsync(new AccessLog
            {
                Id = Guid.NewGuid(),
                UserId = user?.Id,
                PartnerId = user?.PartnerId,
                Action = "LOGIN_FAILED",
                Resource = "auth/login",
                IpAddress = ip,
                UserAgent = userAgent,
                Success = false,
                Details = "Credenciais invalidas.",
                CreatedAt = DateTime.UtcNow
            });

            throw new UnauthorizedAccessException("E-mail ou senha invalidos.");
        }

        var roles = await _userRoleRepository.GetRolesByUserIdAsync(user.Id);
        var role = roles.FirstOrDefault()?.Name ?? user.UserType;

        var sessionId = Guid.NewGuid();
        var jti = Guid.NewGuid().ToString();

        var accessToken = _jwtTokenService.GenerateAccessToken(
            user,
            role,
            user.PartnerId,
            "platform_account",
            jti,
            sessionId);

        var refreshTokenValue = _refreshTokenService.GenerateRefreshToken();

        var session = new Session
        {
            Id = sessionId,
            UserId = user.Id,
            PartnerCustomerId = null,
            AccessTokenJti = jti,
            IpAddress = ip,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsValid = true
        };

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            UserId = user.Id,
            PartnerCustomerId = null,
            Token = refreshTokenValue,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(dto.RememberMe ? 30 : 7),
            CreatedByIp = ip
        };

        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _sessionRepository.CreateAsync(session);
        await _refreshTokenRepository.CreateAsync(refreshToken);
        await _userRepository.UpdateAsync(user);

        await _accessLogRepository.CreateAsync(new AccessLog
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PartnerId = user.PartnerId,
            SessionId = sessionId,
            Action = "LOGIN_SUCCESS",
            Resource = "auth/login",
            IpAddress = ip,
            UserAgent = userAgent,
            Success = true,
            Details = "Login realizado com sucesso.",
            CreatedAt = DateTime.UtcNow
        });

        return new AuthenticateResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresIn = 900,
            SessionId = sessionId.ToString(),
            User = new AuthenticatedUserDto
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                Role = role,
                PartnerId = user.PartnerId?.ToString(),
                AccountOrigin = "platform_account",
                SessionId = sessionId.ToString()
            }
        };
    }

    public async Task<AuthenticateResultDto> LoginWithPartnerCodeAsync(PartnerCodeLoginDto dto)
    {
        await _partnerCodeValidator.ValidateAndThrowAsync(dto);

        var ip = _requestContextAccessor.GetIpAddress();
        var userAgent = _requestContextAccessor.GetUserAgent();

        var code = await _partnerAccessCodeRepository.GetByCodeAsync(dto.Code);

        if (code is null)
        {
            await _accessLogRepository.CreateAsync(new AccessLog
            {
                Id = Guid.NewGuid(),
                Action = "PARTNER_CODE_FAILED",
                Resource = "auth/partner-code",
                IpAddress = ip,
                UserAgent = userAgent,
                Success = false,
                Details = "Codigo invalido.",
                CreatedAt = DateTime.UtcNow
            });

            throw new UnauthorizedAccessException("Codigo do parceiro invalido.");
        }

        if (!string.Equals(code.Status, "active", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Codigo do parceiro inativo.");

        if (code.ExpiresAt.HasValue && code.ExpiresAt.Value < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Codigo do parceiro expirado.");

        if (code.MaxUses.HasValue && code.UsedCount >= code.MaxUses.Value)
            throw new UnauthorizedAccessException("Codigo do parceiro atingiu o limite de uso.");

        var customer = new PartnerCustomer
        {
            Id = Guid.NewGuid(),
            PartnerId = code.PartnerId,
            OriginCodeId = code.Id,
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastAccessAt = DateTime.UtcNow
        };

        await _partnerCustomerRepository.CreateAsync(customer);

        code.UsedCount += 1;
        code.UpdatedAt = DateTime.UtcNow;
        await _partnerAccessCodeRepository.UpdateAsync(code);

        var sessionId = Guid.NewGuid();
        var jti = Guid.NewGuid().ToString();

        var accessToken = _jwtTokenService.GenerateAccessTokenForPartnerCustomer(
            customer,
            code.PartnerId,
            "partner_customer",
            "partner_code",
            jti,
            sessionId);

        var refreshTokenValue = _refreshTokenService.GenerateRefreshToken();

        var session = new Session
        {
            Id = sessionId,
            UserId = null,
            PartnerCustomerId = customer.Id,
            AccessTokenJti = jti,
            IpAddress = ip,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsValid = true
        };

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            UserId = null,
            PartnerCustomerId = customer.Id,
            Token = refreshTokenValue,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ip
        };

        await _sessionRepository.CreateAsync(session);
        await _refreshTokenRepository.CreateAsync(refreshToken);

        await _accessLogRepository.CreateAsync(new AccessLog
        {
            Id = Guid.NewGuid(),
            PartnerCustomerId = customer.Id,
            PartnerId = customer.PartnerId,
            SessionId = sessionId,
            Action = "PARTNER_CODE_SUCCESS",
            Resource = "auth/partner-code",
            IpAddress = ip,
            UserAgent = userAgent,
            Success = true,
            Details = "Acesso por codigo realizado com sucesso.",
            CreatedAt = DateTime.UtcNow
        });

        return new AuthenticateResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresIn = 900,
            SessionId = sessionId.ToString(),
            User = new AuthenticatedUserDto
            {
                Id = customer.Id.ToString(),
                Name = customer.Name,
                Email = customer.Email,
                Role = "partner_customer",
                PartnerId = customer.PartnerId.ToString(),
                AccountOrigin = "partner_code",
                SessionId = sessionId.ToString()
            }
        };
    }

    public async Task<AuthenticateResultDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        await _refreshTokenValidator.ValidateAndThrowAsync(dto);

        var ip = _requestContextAccessor.GetIpAddress();
        var userAgent = _requestContextAccessor.GetUserAgent();

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken);

        if (storedToken is null)
            throw new UnauthorizedAccessException("Refresh token invalido.");

        if (storedToken.RevokedAt.HasValue)
            throw new UnauthorizedAccessException("Refresh token revogado.");

        if (storedToken.ExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expirado.");

        var session = await _sessionRepository.GetByIdAsync(storedToken.SessionId);
        if (session is null || !session.IsValid)
            throw new UnauthorizedAccessException("SessÃ£o invalida.");

        var newJti = Guid.NewGuid().ToString();
        var newRefreshTokenValue = _refreshTokenService.GenerateRefreshToken();

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByToken = newRefreshTokenValue;
        await _refreshTokenRepository.UpdateAsync(storedToken);

        session.AccessTokenJti = newJti;
        session.ExpiresAt = DateTime.UtcNow.AddMinutes(15);
        session.IsValid = true;
        session.IpAddress = ip ?? session.IpAddress;
        session.UserAgent = userAgent ?? session.UserAgent;
        await _sessionRepository.UpdateAsync(session);

        if (storedToken.UserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(storedToken.UserId.Value)
                       ?? throw new UnauthorizedAccessException("Usuario nao encontrado.");

            var roles = await _userRoleRepository.GetRolesByUserIdAsync(user.Id);
            var role = roles.FirstOrDefault()?.Name ?? user.UserType;

            var accessToken = _jwtTokenService.GenerateAccessToken(
                user,
                role,
                user.PartnerId,
                "platform_account",
                newJti,
                session.Id);

            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                UserId = user.Id,
                PartnerCustomerId = null,
                Token = newRefreshTokenValue,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ip
            };

            await _refreshTokenRepository.CreateAsync(newRefreshToken);

            await _accessLogRepository.CreateAsync(new AccessLog
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                PartnerId = user.PartnerId,
                SessionId = session.Id,
                Action = "REFRESH_SUCCESS",
                Resource = "auth/refresh",
                IpAddress = ip,
                UserAgent = userAgent,
                Success = true,
                Details = "Refresh token renovado com sucesso.",
                CreatedAt = DateTime.UtcNow
            });

            return new AuthenticateResultDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenValue,
                ExpiresIn = 900,
                SessionId = session.Id.ToString(),
                User = new AuthenticatedUserDto
                {
                    Id = user.Id.ToString(),
                    Name = user.Name,
                    Email = user.Email,
                    Role = role,
                    PartnerId = user.PartnerId?.ToString(),
                    AccountOrigin = "platform_account",
                    SessionId = session.Id.ToString()
                }
            };
        }

        if (storedToken.PartnerCustomerId.HasValue)
        {
            var customer = await _partnerCustomerRepository.GetByIdAsync(storedToken.PartnerCustomerId.Value)
                           ?? throw new UnauthorizedAccessException("Cliente do parceiro nao encontrado.");

            var accessToken = _jwtTokenService.GenerateAccessTokenForPartnerCustomer(
                customer,
                customer.PartnerId,
                "partner_customer",
                "partner_code",
                newJti,
                session.Id);

            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                UserId = null,
                PartnerCustomerId = customer.Id,
                Token = newRefreshTokenValue,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ip
            };

            await _refreshTokenRepository.CreateAsync(newRefreshToken);

            await _accessLogRepository.CreateAsync(new AccessLog
            {
                Id = Guid.NewGuid(),
                PartnerCustomerId = customer.Id,
                PartnerId = customer.PartnerId,
                SessionId = session.Id,
                Action = "REFRESH_SUCCESS",
                Resource = "auth/refresh",
                IpAddress = ip,
                UserAgent = userAgent,
                Success = true,
                Details = "Refresh token renovado com sucesso.",
                CreatedAt = DateTime.UtcNow
            });

            return new AuthenticateResultDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenValue,
                ExpiresIn = 900,
                SessionId = session.Id.ToString(),
                User = new AuthenticatedUserDto
                {
                    Id = customer.Id.ToString(),
                    Name = customer.Name,
                    Email = customer.Email,
                    Role = "partner_customer",
                    PartnerId = customer.PartnerId.ToString(),
                    AccountOrigin = "partner_code",
                    SessionId = session.Id.ToString()
                }
            };
        }

        throw new UnauthorizedAccessException("Refresh token sem ator associado.");
    }

    public async Task LogoutAsync(LogoutRequestDto dto)
    {
        await _logoutValidator.ValidateAndThrowAsync(dto);

        var ip = _requestContextAccessor.GetIpAddress();
        var userAgent = _requestContextAccessor.GetUserAgent();

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken);
        if (storedToken is null)
            return;

        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(storedToken);
        await _sessionRepository.RevokeByIdAsync(storedToken.SessionId, DateTime.UtcNow);

        await _accessLogRepository.CreateAsync(new AccessLog
        {
            Id = Guid.NewGuid(),
            UserId = storedToken.UserId,
            PartnerCustomerId = storedToken.PartnerCustomerId,
            SessionId = storedToken.SessionId,
            Action = "LOGOUT",
            Resource = "auth/logout",
            IpAddress = ip,
            UserAgent = userAgent,
            Success = true,
            Details = "Logout realizado com sucesso.",
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<MeResponseDto> GetMeAsync(string? userId, string? role, string? partnerId, string? email, string? sessionId, string? origin)
    {
        var response = new MeResponseDto
        {
            Authenticated = !string.IsNullOrWhiteSpace(userId),
            User = string.IsNullOrWhiteSpace(userId)
                ? null
                : new AuthenticatedUserDto
                {
                    Id = userId!,
                    Email = email,
                    Role = role ?? string.Empty,
                    PartnerId = partnerId,
                    AccountOrigin = origin ?? string.Empty,
                    SessionId = sessionId
                }
        };

        return Task.FromResult(response);
    }
}
