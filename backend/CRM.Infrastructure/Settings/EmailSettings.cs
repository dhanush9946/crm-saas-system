namespace CRM.Infrastructure.Settings;

public sealed class EmailSettings
{
    public string Host { get; set; } = default!;

    public int Port { get; set; }

    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string FromEmail { get; set; } = default!;

    public string FromName { get; set; } = default!;

    public string VerificationUrl { get; set; } = default!;

    public string PasswordResetUrl { get; set; } = default!;
}
