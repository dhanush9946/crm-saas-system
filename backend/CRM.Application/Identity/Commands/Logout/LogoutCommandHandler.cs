using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Identity.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LogoutCommandHandler> _logger;
        private readonly IAuditService _auditService;

        public LogoutCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IRefreshTokenService refreshTokenService,
            IUnitOfWork unitOfWork,
            ILogger<LogoutCommandHandler> logger,
            IAuditService auditService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenService = refreshTokenService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _auditService = auditService;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var tokenHash = _refreshTokenService.Hash(request.RefreshToken);
            var existingToken = await _refreshTokenRepository.GetByHashAsync(tokenHash, cancellationToken);

            if (existingToken == null)
            {
                _logger.LogInformation(
                    "Logout requested with unknown refresh token. IP: {IpAddress}, DeviceId: {DeviceId}",
                    request.IpAddress,
                    request.DeviceId);

                return;
            }

            if (existingToken.TenantId == Guid.Empty || existingToken.UserId == Guid.Empty)
            {
                _logger.LogWarning(
                    "Logout skipped because refresh token has invalid identity data. UserId: {UserId}, TenantId: {TenantId}, IP: {IpAddress}, DeviceId: {DeviceId}",
                    existingToken.UserId,
                    existingToken.TenantId,
                    request.IpAddress,
                    request.DeviceId);

                return;
            }

            if (!existingToken.IsActive())
            {
                _logger.LogInformation(
                    "Logout requested for inactive refresh token. UserId: {UserId}, TenantId: {TenantId}, IP: {IpAddress}, DeviceId: {DeviceId}",
                    existingToken.UserId,
                    existingToken.TenantId,
                    request.IpAddress,
                    request.DeviceId);

                return;
            }

            var tokenFamily = await _refreshTokenRepository.GetByFamilyIdAsync(
                existingToken.TokenFamilyId,
                cancellationToken);

            foreach (var token in tokenFamily.Where(token => token.IsActive()))
            {
                token.Revoke();
            }

            _logger.LogInformation(
                "User logged out. UserId: {UserId}, TenantId: {TenantId}, IP: {IpAddress}, DeviceId: {DeviceId}",
                existingToken.UserId,
                existingToken.TenantId,
                request.IpAddress,
                request.DeviceId);

            await _auditService.LogAsync(
                AuditActionConstants.LogoutSucceeded,
                existingToken.UserId,
                existingToken.TenantId,
                "RefreshToken",
                existingToken.Id.ToString(),
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                deviceId: request.DeviceId,
                traceId: request.TraceId,
                metadataJson: $$"""{"sessionId":"{{existingToken.TokenFamilyId}}","revokedTokenCount":{{tokenFamily.Count(token => token.IsRevoked())}}}""",
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
