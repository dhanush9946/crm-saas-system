namespace CRM.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default);

    Task SendVerificationEmailAsync(
        string to,
        string rawToken,
        CancellationToken cancellationToken = default);

    Task SendPasswordResetEmailAsync(
        string to,
        string tenantSlug,
        string rawToken,
        CancellationToken cancellationToken = default);

    Task SendPasswordResetConfirmationEmailAsync(
        string to,
        CancellationToken cancellationToken = default);
}
