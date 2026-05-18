using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Interfaces.Persistence;
using CRM.Domain.Identity.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Identity.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler
    : IRequestHandler<VerifyEmailCommand>
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly ILogger<VerifyEmailCommandHandler> _logger;

    public VerifyEmailCommandHandler(
        ITokenGenerator tokenGenerator,
        IEmailVerificationTokenRepository emailVerificationTokenRepository,
        IUnitOfWork unitOfWork,
        IAuditService auditService,
        ILogger<VerifyEmailCommandHandler> logger)
    {
        _tokenGenerator = tokenGenerator;
        _emailVerificationTokenRepository = emailVerificationTokenRepository;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task Handle(
        VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        // Hash incoming raw token
        var hashedToken = _tokenGenerator.ComputeSha256Hash(request.Token);

        // Find token
        var verificationToken = await _emailVerificationTokenRepository
            .GetByTokenHashAsync(hashedToken, cancellationToken);

        // Validate token exists
        if (verificationToken is null)
        {
            _logger.LogWarning(
                "Email verification failed: Invalid/unknown token. IP: {IpAddress}, DeviceId: {DeviceId}",
                request.IpAddress,
                request.DeviceId);

            await _auditService.LogAsync(
                AuditActionConstants.EmailVerificationFailed,
                succeeded: false,
                failureReason: "Invalid verification token.",
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                deviceId: request.DeviceId,
                traceId: request.TraceId,
                cancellationToken: cancellationToken);

            throw new BadRequestException("Invalid verification token.");
        }

        // Validate token not already used
        if (verificationToken.IsUsed())
        {
            _logger.LogWarning(
                "Email verification failed: Token already used. UserId: {UserId}, TenantId: {TenantId}, IP: {IpAddress}, DeviceId: {DeviceId}",
                verificationToken.UserId,
                verificationToken.TenantId,
                request.IpAddress,
                request.DeviceId);

            await _auditService.LogAsync(
                AuditActionConstants.EmailVerificationFailed,
                userId: verificationToken.UserId,
                tenantId: verificationToken.TenantId,
                entityType: "EmailVerificationToken",
                entityId: verificationToken.Id.ToString(),
                succeeded: false,
                failureReason: "Verification token already used.",
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                deviceId: request.DeviceId,
                traceId: request.TraceId,
                cancellationToken: cancellationToken);

            throw new BadRequestException("Verification token already used.");
        }

        // Validate token not expired
        if (verificationToken.IsExpired())
        {
            _logger.LogWarning(
                "Email verification failed: Token expired. UserId: {UserId}, TenantId: {TenantId}, IP: {IpAddress}, DeviceId: {DeviceId}",
                verificationToken.UserId,
                verificationToken.TenantId,
                request.IpAddress,
                request.DeviceId);

            await _auditService.LogAsync(
                AuditActionConstants.EmailVerificationFailed,
                userId: verificationToken.UserId,
                tenantId: verificationToken.TenantId,
                entityType: "EmailVerificationToken",
                entityId: verificationToken.Id.ToString(),
                succeeded: false,
                failureReason: "Verification token expired.",
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                deviceId: request.DeviceId,
                traceId: request.TraceId,
                cancellationToken: cancellationToken);

            throw new BadRequestException("Verification token expired.");
        }

        // Mark email verified
        verificationToken.User.MarkEmailVerified();

        // Mark token used
        verificationToken.MarkAsUsed();

        _logger.LogInformation(
            "Email successfully verified for UserId: {UserId}, TenantId: {TenantId}, IP: {IpAddress}, DeviceId: {DeviceId}",
            verificationToken.UserId,
            verificationToken.TenantId,
            request.IpAddress,
            request.DeviceId);

        await _auditService.LogAsync(
            AuditActionConstants.EmailVerified,
            userId: verificationToken.UserId,
            tenantId: verificationToken.TenantId,
            entityType: "User",
            entityId: verificationToken.UserId.ToString(),
            succeeded: true,
            ipAddress: request.IpAddress,
            userAgent: request.UserAgent,
            deviceId: request.DeviceId,
            traceId: request.TraceId,
            metadataJson: $$"""{"tokenId":"{{verificationToken.Id}}"}""",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}