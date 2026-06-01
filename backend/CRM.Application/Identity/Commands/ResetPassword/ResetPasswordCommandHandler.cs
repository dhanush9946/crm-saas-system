using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Interfaces.Persistence;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Identity.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        ITenantRepository tenantRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IRefreshTokenService refreshTokenService,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IAuditService auditService,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _refreshTokenService = refreshTokenService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetBySlugAsync(
            request.TenantSlug.Trim(),
            cancellationToken);

        if (tenant is null)
        {
            await _auditService.LogAsync(
                action: AuditActionConstants.PasswordResetFailed,
                succeeded: false,
                failureReason: "Invalid tenant.",
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                deviceId: request.DeviceId,
                traceId: request.TraceId,
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new BadRequestException("Invalid or expired reset token.");
        }

        var tokenHash = _tokenGenerator.ComputeSha256Hash(request.Token);

        var resetToken = await _passwordResetTokenRepository.GetUsableByHashAsync(
            tenant.Id,
            tokenHash,
            cancellationToken);

        if (resetToken is null || !resetToken.IsUsable())
        {
            await _auditService.LogAsync(
                action: AuditActionConstants.PasswordResetFailed,
                tenantId: tenant.Id,
                succeeded: false,
                failureReason: "Invalid/expired/revoked reset token.",
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                deviceId: request.DeviceId,
                traceId: request.TraceId,
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new BadRequestException("Invalid or expired reset token.");
        }

        var user = resetToken.User;
        user.SetPasswordHash(_passwordHasher.Hash(request.NewPassword));
        user.IncrementTokenVersion();

        resetToken.MarkAsUsed();

        await _passwordResetTokenRepository.RevokeActiveByUserAsync(
            tenant.Id,
            user.Id,
            cancellationToken);

        var revokedSessionCount = await _refreshTokenService.RevokeAllByUserAsync(
            tenant.Id,
            user.Id,
            cancellationToken);

        await _auditService.LogAsync(
            action: AuditActionConstants.PasswordResetSucceeded,
            userId: user.Id,
            tenantId: tenant.Id,
            entityType: "User",
            entityId: user.Id.ToString(),
            ipAddress: request.IpAddress,
            userAgent: request.UserAgent,
            deviceId: request.DeviceId,
            traceId: request.TraceId,
            metadataJson: $$"""{"resetTokenId":"{{resetToken.Id}}","revokedSessionCount":{{revokedSessionCount}}}""",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _emailService.SendPasswordResetConfirmationEmailAsync(
                user.Email,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send password reset confirmation email to {Email} for UserId: {UserId}",
                user.Email,
                user.Id);
        }

        _logger.LogInformation(
            "Password reset succeeded for UserId: {UserId}, TenantId: {TenantId}. Revoked sessions: {RevokedSessionCount}",
            user.Id,
            tenant.Id,
            revokedSessionCount);
    }
}
