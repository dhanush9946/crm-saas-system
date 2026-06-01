using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Interfaces.Persistence;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Constants;
using CRM.Domain.Identity.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Identity.Commands.ResendVerificationEmail;

public sealed class ResendVerificationEmailCommandHandler
    : IRequestHandler<ResendVerificationEmailCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly ILogger<ResendVerificationEmailCommandHandler> _logger;

    public ResendVerificationEmailCommandHandler(
        IUserRepository userRepository,
        IEmailVerificationTokenRepository emailVerificationTokenRepository,
        ITokenGenerator tokenGenerator,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IAuditService auditService,
        ILogger<ResendVerificationEmailCommandHandler> logger)
    {
        _userRepository = userRepository;
        _emailVerificationTokenRepository = emailVerificationTokenRepository;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task Handle(
        ResendVerificationEmailCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        var user = await _userRepository.GetByNormalizedEmailAsync(
            normalizedEmail,
            cancellationToken);

        // SECURITY:
        // Always return success-like behavior to prevent user enumeration
        if (user is null)
        {
            _logger.LogInformation(
                "Resend verification email requested for non-existent user: {Email}",
                request.Email);
            return;
        }

        if (user.IsEmailVerified)
        {
            _logger.LogInformation(
                "Resend verification email requested for already verified user: {UserId}",
                user.Id);
            return;
        }

        // Revoke old active tokens
        var activeTokens = await _emailVerificationTokenRepository
            .GetActiveTokensByUserIdAsync(
                user.Id,
                cancellationToken);

        foreach (var token in activeTokens)
        {
            token.MarkAsUsed();
        }

        // Generate new token
        var rawToken = _tokenGenerator.GenerateSecureToken();
        var hashedToken = _tokenGenerator.ComputeSha256Hash(rawToken);

        // Create entity
        var verificationToken = EmailVerificationToken.Create(
            user.TenantId,
            user.Id,
            hashedToken,
            DateTime.UtcNow.AddHours(24));

        await _emailVerificationTokenRepository.AddAsync(
            verificationToken,
            cancellationToken);

        // Audit log (registered in context, saved in single transaction below)
        await _auditService.LogAsync(
            action: AuditActionConstants.VerificationEmailResent,
            userId: user.Id,
            tenantId: user.TenantId,
            entityType: "User",
            entityId: user.Id.ToString(),
            ipAddress: request.IpAddress,
            userAgent: request.UserAgent,
            deviceId: request.DeviceId,
            traceId: request.TraceId,
            metadataJson: $$"""{"tokenId":"{{verificationToken.Id}}"}""",
            cancellationToken: cancellationToken);

        // SINGLE SAVE ALL
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send email (Resilient, non-blocking for registration/resend flow)
        try
        {
            await _emailService.SendVerificationEmailAsync(
                user.Email,
                rawToken,
                cancellationToken);

            _logger.LogInformation(
                "Verification email successfully resent to {Email} for UserId: {UserId}",
                user.Email,
                user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to resend email verification token to {Email} for UserId: {UserId}",
                user.Email,
                user.Id);
        }
    }
}