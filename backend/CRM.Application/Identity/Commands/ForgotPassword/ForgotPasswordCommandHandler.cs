using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Interfaces.Persistence;
using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Constants;
using CRM.Domain.Identity.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Identity.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        ITokenGenerator tokenGenerator,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IAuditService auditService,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Security: Always behave success-like to avoid account enumeration.
        var tenant = await _tenantRepository.GetBySlugAsync(
            request.TenantSlug.Trim(),
            cancellationToken);

        if (tenant is null)
        {
            _logger.LogInformation(
                "Forgot password requested for unknown tenant slug: {TenantSlug}",
                request.TenantSlug);
            return;
        }

        var user = await _userRepository.GetByEmailAsync(
            tenant.Id,
            request.Email,
            cancellationToken);

        if (user is null)
        {
            _logger.LogInformation(
                "Forgot password requested for unknown user email: {Email} in TenantId: {TenantId}",
                request.Email,
                tenant.Id);
            return;
        }

        await _passwordResetTokenRepository.RevokeActiveByUserAsync(
            tenant.Id,
            user.Id,
            cancellationToken);

        var rawToken = _tokenGenerator.GenerateSecureToken();
        var tokenHash = _tokenGenerator.ComputeSha256Hash(rawToken);

        var passwordResetToken = PasswordResetToken.Create(
            tenant.Id,
            user.Id,
            tokenHash,
            DateTime.UtcNow.AddHours(1),
            request.IpAddress,
            request.UserAgent);

        await _passwordResetTokenRepository.AddAsync(
            passwordResetToken,
            cancellationToken);

        await _auditService.LogAsync(
            action: AuditActionConstants.PasswordResetRequested,
            userId: user.Id,
            tenantId: tenant.Id,
            entityType: nameof(PasswordResetToken),
            entityId: passwordResetToken.Id.ToString(),
            ipAddress: request.IpAddress,
            userAgent: request.UserAgent,
            deviceId: request.DeviceId,
            traceId: request.TraceId,
            metadataJson: $$"""{"email":"{{user.Email}}"}""",
            cancellationToken: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _emailService.SendPasswordResetEmailAsync(
                user.Email,
                tenant.Slug,
                rawToken,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send password reset email to {Email} for UserId: {UserId}",
                user.Email,
                user.Id);
        }
    }
}
