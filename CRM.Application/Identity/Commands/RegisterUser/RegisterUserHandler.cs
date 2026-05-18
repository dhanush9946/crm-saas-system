using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Interfaces.Persistence;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Constants;
using CRM.Domain.Identity.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Identity.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IAuditService _auditService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailVerificationTokenRepository
                                  _emailVerificationTokenRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<RegisterUserHandler> _logger;

        public RegisterUserHandler(
            IUserRepository userRepository,
            ITenantRepository tenantRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenService refreshTokenService,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IAuditService auditService,
            ITokenGenerator tokenGenerator,
            IEmailVerificationTokenRepository emailVerificationTokenRepository,
            IEmailService emailService,
            ILogger<RegisterUserHandler> logger)
        {
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenService = refreshTokenService;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _auditService = auditService;
            _tokenGenerator = tokenGenerator;
            _emailVerificationTokenRepository =
                emailVerificationTokenRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<AuthResponseDto> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Check tenant already exists
            var existingTenant = await _tenantRepository
                .GetBySlugAsync(request.TenantSlug, cancellationToken);

            if (existingTenant != null)
                throw new ConflictException("Tenant already exists");

            // 2. Create Tenant
            var tenant = Tenant.Create(request.TenantName, request.TenantSlug);

            // 3. Create User
            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = User.Create(
                tenant.Id,
                request.Email,
                request.DisplayName
            );

            user.SetPasswordHash(passwordHash);

            //email verification token generation
            var rawVerificationToken =
                _tokenGenerator.GenerateSecureToken();

            var hashedVerificationToken =
                _tokenGenerator.ComputeSha256Hash(
                    rawVerificationToken);

            var verificationToken =
                EmailVerificationToken.Create(
                    user.TenantId,
                    user.Id,
                    hashedVerificationToken,
                    DateTime.UtcNow.AddHours(24));

            await _emailVerificationTokenRepository
                            .AddAsync(
                                verificationToken,
                                cancellationToken);


            // 4. Ensure OWNER role exists
            const string roleName = RoleConstants.Owner;
            var normalized = roleName.ToUpperInvariant();

            var ownerRole = await _roleRepository
                .GetByNameAsync(tenant.Id, normalized);

            if (ownerRole == null)
            {
                ownerRole = Role.Create(tenant.Id, roleName, true);
                await _roleRepository.AddAsync(ownerRole, cancellationToken);
            }

            // 5. Assign role to user
            var exists = await _userRoleRepository
                .ExistsAsync(tenant.Id, user.Id, ownerRole.Id);

            if (!exists)
            {
                var userRole = UserRole.Create(
                    tenant.Id,
                    user.Id,
                    ownerRole.Id
                );

                await _userRoleRepository.AddAsync(userRole, cancellationToken);
            }

            // 6. Add entities (NO SAVE YET)
            await _tenantRepository.AddAsync(tenant, cancellationToken);
            await _userRepository.AddAsync(user, cancellationToken);


            // 8. Generate tokens
            var roles = new List<string> { roleName };

            var refreshToken = await _refreshTokenService.CreateAsync(
                    user.TenantId,
                    user.Id,
                    request.DeviceId,
                    request.UserAgent,
                    request.IpAddress,
                    cancellationToken
                );

            var accessToken = _jwtService.GenerateToken(
                user.Id,
                user.TenantId,
                refreshToken.SessionId,
                user.Email,
                user.TokenVersion,
                roles);

            await _auditService.LogAsync(
                AuditActionConstants.TenantRegistered,
                user.Id,
                tenant.Id,
                nameof(Tenant),
                tenant.Id.ToString(),
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                deviceId: request.DeviceId,
                traceId: request.TraceId,
                metadataJson: $$"""{"sessionId":"{{refreshToken.SessionId}}","ownerRoleId":"{{ownerRole.Id}}"}""",
                cancellationToken: cancellationToken);

            // 9. SINGLE SAVE ALL
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 10. Send Verification Email (Catch & Log errors so SMTP failures don't fail registration)
            try
            {
                await _emailService.SendVerificationEmailAsync(
                    user.Email,
                    rawVerificationToken,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send email verification token to {Email} for UserId: {UserId}",
                    user.Email,
                    user.Id);
            }

            // 11. Return response
            return new AuthResponseDto
            {
                TenantId = tenant.Id,
                UserId = user.Id,
                SessionId = refreshToken.SessionId,
                AccessToken = accessToken,
                RefreshToken = refreshToken.RawToken
            };
        }
    }
}
