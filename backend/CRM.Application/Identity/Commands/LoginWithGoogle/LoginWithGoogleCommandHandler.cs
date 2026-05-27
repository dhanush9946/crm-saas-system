using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Constants;
using MediatR;

namespace CRM.Application.Identity.Commands.LoginWithGoogle;

public sealed class LoginWithGoogleCommandHandler
    : IRequestHandler<LoginWithGoogleCommand, LoginWithGoogleResponse>
{
    private readonly IExternalLoginRepository _externalLoginRepository;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public LoginWithGoogleCommandHandler(
        IExternalLoginRepository externalLoginRepository,
        IGoogleAuthService googleAuthService,
        IUserRoleRepository userRoleRepository,
        IRefreshTokenService refreshTokenService,
        IJwtService jwtService,
        IUnitOfWork unitOfWork,
        IAuditService auditService)
    {
        _externalLoginRepository = externalLoginRepository;
        _googleAuthService = googleAuthService;
        _userRoleRepository = userRoleRepository;
        _refreshTokenService = refreshTokenService;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<LoginWithGoogleResponse> Handle(
        LoginWithGoogleCommand request,
        CancellationToken cancellationToken)
    {
        var googleUser = await _googleAuthService.ValidateAsync(
            request.IdToken,
            cancellationToken);

        var externalLogin = await _externalLoginRepository.GetByProviderAsync(
            ExternalLoginProviders.Google,
            googleUser.Subject,
            cancellationToken);

        if (externalLogin is null)
        {
            return new LoginWithGoogleResponse
            {
                RequiresOnboarding = true,
                Email = googleUser.Email,
                FullName = googleUser.Name
            };
        }

        var user = externalLogin.User;

        if (user.IsDisabled())
        {
            await _auditService.LogAsync(
                AuditActionConstants.LoginFailed,
                user.Id,
                user.TenantId,
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

        var roles = await _userRoleRepository.GetUserRolesAsync(
            user.TenantId,
            user.Id,
            cancellationToken);

        var refreshToken = await _refreshTokenService.CreateAsync(
            user.TenantId,
            user.Id,
            request.DeviceId,
            request.UserAgent,
            request.IpAddress,
            cancellationToken);

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
            metadataJson: $$"""{"sessionId":"{{refreshToken.SessionId}}","provider":"{{ExternalLoginProviders.Google}}"}""",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginWithGoogleResponse
        {
            RequiresOnboarding = false,
            AccessToken = accessToken,
            RefreshToken = refreshToken.RawToken,
            SessionId = refreshToken.SessionId,
            UserId = user.Id,
            TenantId = user.TenantId,
            Email = user.Email,
            FullName = user.DisplayName ?? user.Email
        };
    }
}
