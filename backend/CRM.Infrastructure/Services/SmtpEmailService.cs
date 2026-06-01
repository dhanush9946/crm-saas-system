using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CRM.Infrastructure.Services;

public sealed class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public SmtpEmailService(
        IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();

        email.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

        email.To.Add(MailboxAddress.Parse(to));

        email.Subject = subject;

        email.Body = new TextPart("html")
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);

        await smtp.AuthenticateAsync(
            _settings.Username,
            _settings.Password,
            cancellationToken);

        await smtp.SendAsync(
            email,
            cancellationToken);

        await smtp.DisconnectAsync(
            true,
            cancellationToken);
    }

    public async Task SendVerificationEmailAsync(
        string to,
        string rawToken,
        CancellationToken cancellationToken = default)
    {
        var verificationLink = $"{_settings.VerificationUrl}?token={Uri.EscapeDataString(rawToken)}";
        var subject = "Verify your email";
        var body = $"""
            <h2>Email Verification</h2>
            <p>Please verify your email by clicking below:</p>
            <a href="{verificationLink}">
                Verify Email
            </a>
            """;

        await SendEmailAsync(to, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(
        string to,
        string tenantSlug,
        string rawToken,
        CancellationToken cancellationToken = default)
    {
        var resetLink =
            $"{_settings.PasswordResetUrl}?token={Uri.EscapeDataString(rawToken)}&tenant={Uri.EscapeDataString(tenantSlug)}";
        var subject = "Reset your password";
        var body = $"""
            <h2>Password Reset Request</h2>
            <p>Use the link below to reset your password. If you did not request this, you can safely ignore this email.</p>
            <a href="{resetLink}">
                Reset Password
            </a>
            """;

        await SendEmailAsync(to, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetConfirmationEmailAsync(
        string to,
        CancellationToken cancellationToken = default)
    {
        var subject = "Your password was changed";
        var body = """
            <h2>Password Updated</h2>
            <p>Your password was changed successfully.</p>
            <p>If you did not perform this action, contact support immediately.</p>
            """;

        await SendEmailAsync(to, subject, body, cancellationToken);
    }
}
