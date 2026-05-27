using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Constants;
using CRM.Domain.Identity.Entities;
using MediatR;

namespace CRM.Application.Identity.Commands.CompleteGoogleOnboarding
{
    public class CompleteGoogleOnboardingCommandHandler
        : IRequestHandler<CompleteGoogleOnboardingCommand, CompleteGoogleOnboardingResponse>
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IExternalLoginRepository _externalLoginRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;

        public CompleteGoogleOnboardingCommandHandler(
            IGoogleAuthService googleAuthService,
            ITenantRepository tenantRepository,
            IUserRepository userRepository,
            IExternalLoginRepository externalLoginRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IRefreshTokenService refreshTokenService,
            IJwtService jwtService,
            IUnitOfWork unitOfWork,
            IAuditService auditService)
        {
            _googleAuthService = googleAuthService;
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _externalLoginRepository = externalLoginRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _refreshTokenService = refreshTokenService;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _auditService = auditService;
        }

        public async Task<CompleteGoogleOnboardingResponse> Handle(
            CompleteGoogleOnboardingCommand request,
            CancellationToken cancellationToken)
        {
            var googleUser = await _googleAuthService.ValidateAsync(
                request.IdToken,
                cancellationToken);

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var existingExternalLogin =
                    await _externalLoginRepository.GetByProviderAsync(
                        ExternalLoginProviders.Google,
                        googleUser.Subject,
                        cancellationToken);

                if (existingExternalLogin is not null)
                    throw new ConflictException("Google account is already linked.");

                var existingTenant = await _tenantRepository.GetBySlugAsync(
                    request.TenantSlug,
                    cancellationToken);

                if (existingTenant is not null)
                    throw new ConflictException("Tenant already exists");

                var tenant = Tenant.Create(
                    request.TenantName,
                    request.TenantSlug);

                var user = User.Create(
                    tenant.Id,
                    googleUser.Email,
                    googleUser.Name);

                if (googleUser.EmailVerified)
                {
                    user.MarkEmailVerified();
                }

                const string roleName = RoleConstants.Owner;

                var ownerRole = Role.Create(tenant.Id, roleName, true);

                var userRole = UserRole.Create(
                    tenant.Id,
                    user.Id,
                    ownerRole.Id);

                var externalLogin = ExternalLogin.Create(
                    tenant.Id,
                    user.Id,
                    ExternalLoginProviders.Google,
                    googleUser.Subject,
                    googleUser.Email);

                await _tenantRepository.AddAsync(tenant, cancellationToken);
                await _userRepository.AddAsync(user, cancellationToken);
                await _roleRepository.AddAsync(ownerRole, cancellationToken);
                await _userRoleRepository.AddAsync(userRole, cancellationToken);
                await _externalLoginRepository.AddAsync(
                    externalLogin,
                    cancellationToken);

                var refreshToken = await _refreshTokenService.CreateAsync(
                    user.TenantId,
                    user.Id,
                    request.DeviceId,
                    request.UserAgent,
                    request.IpAddress,
                    cancellationToken);

                var roles = new List<string> { roleName };

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
                    metadataJson: $$"""{"sessionId":"{{refreshToken.SessionId}}","provider":"{{ExternalLoginProviders.Google}}","ownerRoleId":"{{ownerRole.Id}}"}""",
                    cancellationToken: cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return new CompleteGoogleOnboardingResponse
                {
                    TenantId = tenant.Id,
                    UserId = user.Id,
                    SessionId = refreshToken.SessionId,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.RawToken
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
