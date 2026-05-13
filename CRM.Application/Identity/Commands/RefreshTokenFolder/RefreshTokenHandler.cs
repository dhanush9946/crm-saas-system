

using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Interfaces;
using CRM.Application.Common.Interfaces;
using CRM.Domain.Identity.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Identity.Commands.RefreshTokenFolder
{
    public class RefreshTokenHandler:IRequestHandler<RefreshTokenCommand,AuthResponseDto>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RefreshTokenHandler> _logger;
        private readonly IAuditService _auditService;

        public RefreshTokenHandler(
            IRefreshTokenService refreshTokenService,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IJwtService jwtService,
            IUnitOfWork unitOfWork,
            ILogger<RefreshTokenHandler> logger,
            IAuditService auditService)
        {
            _refreshTokenService = refreshTokenService;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _auditService = auditService;
        }

        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request,CancellationToken cancellationToken)
        {
            

            // 2. Get token from DB
            var existingToken = await _refreshTokenService
                .GetByTokenAsync(request.RefreshToken,cancellationToken);

            if (existingToken == null)
                throw new UnauthorizedException("Invalid refresh token");

            // 3. Validate token state
            if (!existingToken.IsActive())
            {
                if (existingToken.IsRevoked() && existingToken.ReplacedByTokenId.HasValue)
                {
                    await _refreshTokenService.RevokeFamilyAsync(
                        existingToken.TokenFamilyId,
                        cancellationToken);

                    await _auditService.LogAsync(
                        AuditActionConstants.RefreshTokenReuseDetected,
                        existingToken.UserId,
                        existingToken.TenantId,
                        "RefreshToken",
                        existingToken.Id.ToString(),
                        succeeded: false,
                        failureReason: "Refresh token reuse detected",
                        ipAddress: request.IpAddress,
                        userAgent: request.UserAgent,
                        deviceId: request.DeviceId,
                        traceId: request.TraceId,
                        metadataJson: $$"""{"tokenFamilyId":"{{existingToken.TokenFamilyId}}"}""",
                        cancellationToken: cancellationToken);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    _logger.LogWarning(
                        "Refresh token reuse detected. TokenId: {TokenId}, TokenFamilyId: {TokenFamilyId}, UserId: {UserId}, TenantId: {TenantId}, IP: {IpAddress}, DeviceId: {DeviceId}",
                        existingToken.Id,
                        existingToken.TokenFamilyId,
                        existingToken.UserId,
                        existingToken.TenantId,
                        request.IpAddress,
                        request.DeviceId);

                    throw new UnauthorizedException("Refresh token reuse detected");
                }

                throw new UnauthorizedException("Refresh token expired or revoked");
            }

            // 4. Get user (for email)
            var user = await _userRepository.GetByIdAsync(existingToken.UserId,cancellationToken);

            if (user == null)
                throw new UnauthorizedException("User not found");

            // 5. Fetch roles
            var roles = await _userRoleRepository.GetUserRolesAsync(user.TenantId, user.Id, cancellationToken);

            // 6. Rotate token
            var newRawToken = await _refreshTokenService.RotateAsync(
                existingToken,
                user.TenantId,
                user.Id,
                request.DeviceId,
                request.UserAgent,
                request.IpAddress,
                cancellationToken
            );

            // 7. Generate new access token for the same session family
            var accessToken = _jwtService.GenerateToken(
                user.Id,
                user.TenantId,
                newRawToken.SessionId,
                user.Email,
                user.TokenVersion,
                roles);

            await _auditService.LogAsync(
                AuditActionConstants.RefreshTokenRotated,
                user.Id,
                user.TenantId,
                "RefreshToken",
                existingToken.Id.ToString(),
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                deviceId: request.DeviceId,
                traceId: request.TraceId,
                metadataJson: $$"""{"sessionId":"{{newRawToken.SessionId}}","tokenFamilyId":"{{existingToken.TokenFamilyId}}"}""",
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                TenantId = user.TenantId,
                UserId = user.Id,
                SessionId = newRawToken.SessionId,
                AccessToken = accessToken,
                RefreshToken = newRawToken.RawToken
            };
        }

    }
}
