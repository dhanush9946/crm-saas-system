using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Identity.Commands.LogoutAll
{
    public class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LogoutAllCommandHandler> _logger;

        public LogoutAllCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            ILogger<LogoutAllCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(LogoutAllCommand request, CancellationToken cancellationToken)
        {
            if (request.TenantId == Guid.Empty || request.UserId == Guid.Empty)
                throw new UnauthorizedException("Invalid user context");

            var activeTokens = await _refreshTokenRepository.GetActiveByUserAsync(
                request.TenantId,
                request.UserId,
                cancellationToken);

            foreach (var token in activeTokens)
            {
                token.Revoke();
            }

            if (activeTokens.Count > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation(
                "LogoutAll audit event. UserId: {UserId}, TenantId: {TenantId}, RevokedSessionCount: {RevokedSessionCount}, IP: {IpAddress}, DeviceId: {DeviceId}",
                request.UserId,
                request.TenantId,
                activeTokens.Count,
                request.IpAddress,
                request.DeviceId);
        }
    }
}
