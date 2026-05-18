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
}