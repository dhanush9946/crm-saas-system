using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Identity.Commands.LogoutAll
{
    public class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LogoutAllCommandHandler> _logger;
        private readonly IAuditService _auditService;

        public LogoutAllCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<LogoutAllCommandHandler> logger,
            IAuditService auditService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _auditService = auditService;
        }

        public async Task Handle(LogoutAllCommand request, CancellationToken cancellationToken)
        {
            if (request.TenantId == Guid.Empty || request.UserId == Guid.Empty)
                throw new UnauthorizedException("Invalid user context");

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null || user.TenantId != request.TenantId)
                throw new UnauthorizedException("Invalid user context");

            var activeTokens = await _refreshTokenRepository.GetActiveByUserAsync(
                request.TenantId,
                request.UserId,
                cancellationToken);

            foreach (var token in activeTokens)
            {
                token.Revoke();
            }

            user.IncrementTokenVersion();

            await _auditService.LogAsync(
                AuditActionConstants.LogoutAllSucceeded,
                request.UserId,
                request.TenantId,
                "User",
                request.UserId.ToString(),
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                traceId: request.TraceId,
                metadataJson: $$"""{"requestSessionId":"{{request.SessionId}}","revokedSessionCount":{{activeTokens.Count}},"tokenVersion":{{user.TokenVersion}}}""",
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "LogoutAll audit event. UserId: {UserId}, TenantId: {TenantId}, RequestSessionId: {SessionId}, RevokedSessionCount: {RevokedSessionCount}, TokenVersion: {TokenVersion}, IP: {IpAddress}",
                request.UserId,
                request.TenantId,
                request.SessionId,
                activeTokens.Count,
                user.TokenVersion,
                request.IpAddress);
        }
    }
}
