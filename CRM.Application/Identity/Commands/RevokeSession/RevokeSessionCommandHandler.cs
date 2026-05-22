using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Constants;
using MediatR;

namespace CRM.Application.Identity.Commands.RevokeSession;

public sealed class RevokeSessionCommandHandler : IRequestHandler<RevokeSessionCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public RevokeSessionCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        IAuditService auditService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task Handle(RevokeSessionCommand request, CancellationToken cancellationToken)
    {
        var tokens = await _refreshTokenRepository.GetByFamilyIdAsync(
            request.SessionId,
            cancellationToken);

        if (!tokens.Any())
        {
            return;
        }

        var belongsToCurrentUser = tokens.All(x =>
            x.UserId == _currentUser.UserId &&
            x.TenantId == _currentUser.TenantId);

        if (!belongsToCurrentUser)
        {
            throw new ForbiddenException("You do not have access to this session.");
        }

        var revokedTokenCount = 0;

        foreach (var token in tokens)
        {
            if (token.IsActive())
            {
                token.Revoke();
                revokedTokenCount++;
            }
        }

        await _auditService.LogAsync(
            AuditActionConstants.SessionRevoked,
            _currentUser.UserId,
            _currentUser.TenantId,
            "RefreshToken",
            request.SessionId.ToString(),
            ipAddress: request.IpAddress,
            userAgent: request.UserAgent,
            traceId: request.TraceId,
            metadataJson: $$"""{"sessionId":"{{request.SessionId}}","revokedTokenCount":{{revokedTokenCount}}}""",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
