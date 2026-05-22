
using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using CRM.Application.Common.Interfaces;
using CRM.Domain.Identity.Constants;
using MediatR;

namespace CRM.Application.Identity.Commands.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand,AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;

        public LoginHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenService refreshTokenService,
            ITenantRepository tenantRepository,
            IUserRoleRepository userRoleRepository,
            IJwtService jwtService,
            IUnitOfWork unitOfWork,
            IAuditService auditService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenService = refreshTokenService;
            _tenantRepository = tenantRepository;
            _userRoleRepository = userRoleRepository;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _auditService = auditService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request,CancellationToken cancellationToken)
        {
            // 1. Get tenant
            var tenant = await _tenantRepository.GetBySlugAsync(request.TenantSlug,cancellationToken);

            if (tenant == null)
            {
                await _auditService.LogAsync(
                    AuditActionConstants.LoginFailed,
                    tenantId: null,
                    succeeded: false,
                    failureReason: "Invalid tenant",
                    ipAddress: request.IpAddress,
                    userAgent: request.UserAgent,
                    deviceId: request.DeviceId,
                    traceId: request.TraceId,
                    cancellationToken: cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new UnauthorizedException("Invalid tenant");
            }

            // 2. Get user
            var user = await _userRepository.GetByEmailAsync(tenant.Id, request.Email,cancellationToken);

            if (user != null && user.IsLockedOut())
            {
                await _auditService.LogAsync(
                    AuditActionConstants.LoginFailed,
                    user.Id,
                    tenant.Id,
                    succeeded: false,
                    failureReason: "Account locked out",
                    ipAddress: request.IpAddress,
                    userAgent: request.UserAgent,
                    deviceId: request.DeviceId,
                    traceId: request.TraceId,
                    cancellationToken: cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new UnauthorizedException("Account is temporarily locked due to multiple failed login attempts.");
            }

            if (user == null || user.PasswordHash == null ||
                !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                if (user != null)
                {
                    user.RecordFailedLogin();
                }

                await _auditService.LogAsync(
                    AuditActionConstants.LoginFailed,
                    user?.Id,
                    tenant.Id,
                    succeeded: false,
                    failureReason: "Invalid credentials",
                    ipAddress: request.IpAddress,
                    userAgent: request.UserAgent,
                    deviceId: request.DeviceId,
                    traceId: request.TraceId,
                    cancellationToken: cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new UnauthorizedException("Invalid credentials");
            }

            // 3. Check disabled
            if (user.IsDisabled())
            {
                await _auditService.LogAsync(
                    AuditActionConstants.LoginFailed,
                    user.Id,
                    tenant.Id,
                    succeeded: false,
                    failureReason: "User is disabled",
                    ipAddress: request.IpAddress,
                    userAgent: request.UserAgent,
                    deviceId: request.DeviceId,
                    traceId: request.TraceId,
                    cancellationToken: cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new UnauthorizedException("User is disabled");
            }

            user.RecordLogin();

            // 4. Fetch roles
            var roles = await _userRoleRepository.GetUserRolesAsync(user.TenantId, user.Id, cancellationToken);

            var refreshToken = await _refreshTokenService.CreateAsync(
                    user.TenantId,
                    user.Id,
                    request.DeviceId,
                    request.UserAgent,
                    request.IpAddress,
                    cancellationToken
                );

            // 5. Generate access token for the created session
            var accessToken = _jwtService.GenerateToken(
                user.Id,
                user.TenantId,
                refreshToken.SessionId,
                user.Email,
                user.TokenVersion,
                roles);

            await _auditService.LogAsync(
                AuditActionConstants.LoginSucceeded,
                user.Id,
                user.TenantId,
                "User",
                user.Id.ToString(),
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                deviceId: request.DeviceId,
                traceId: request.TraceId,
                metadataJson: $$"""{"sessionId":"{{refreshToken.SessionId}}"}""",
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Response
            return new AuthResponseDto
            {
                TenantId = user.TenantId,
                UserId = user.Id,
                SessionId = refreshToken.SessionId,
                AccessToken = accessToken,
                RefreshToken = refreshToken.RawToken
            };
        }
    }
}
