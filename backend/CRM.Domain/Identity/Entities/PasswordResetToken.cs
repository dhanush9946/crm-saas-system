using CRM.Domain.Common;

namespace CRM.Domain.Identity.Entities;

public sealed class PasswordResetToken : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public byte[] TokenHash { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? UsedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    // Navigation
    public User User { get; private set; } = default!;

    private PasswordResetToken() { }

    private PasswordResetToken(
        Guid tenantId,
        Guid userId,
        byte[] tokenHash,
        DateTime expiresAtUtc,
        string? ipAddress,
        string? userAgent)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required", nameof(tenantId));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required", nameof(userId));

        if (tokenHash is null || tokenHash.Length == 0)
            throw new ArgumentException("TokenHash is required", nameof(tokenHash));

        // SHA-256 hash length is 32 bytes.
        if (tokenHash.Length != 32)
            throw new ArgumentException("TokenHash must be a SHA256 hash (32 bytes).", nameof(tokenHash));

        if (expiresAtUtc <= DateTime.UtcNow)
            throw new ArgumentException("ExpiresAtUtc must be in the future.", nameof(expiresAtUtc));

        TenantId = tenantId;
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        IpAddress = string.IsNullOrWhiteSpace(ipAddress) ? null : ipAddress.Trim();
        UserAgent = string.IsNullOrWhiteSpace(userAgent) ? null : userAgent.Trim();
    }

    public static PasswordResetToken Create(
        Guid tenantId,
        Guid userId,
        byte[] tokenHash,
        DateTime expiresAtUtc,
        string? ipAddress = null,
        string? userAgent = null)
    {
        return new PasswordResetToken(tenantId, userId, tokenHash, expiresAtUtc, ipAddress, userAgent);
    }

    public void MarkAsUsed()
    {
        if (UsedAtUtc.HasValue)
            throw new InvalidOperationException("Password reset token has already been used.");

        if (RevokedAtUtc.HasValue)
            throw new InvalidOperationException("Password reset token has been revoked.");

        if (IsExpired())
            throw new InvalidOperationException("Password reset token has expired.");

        UsedAtUtc = DateTime.UtcNow;
        SetUpdated();
    }

    public void Revoke()
    {
        if (RevokedAtUtc.HasValue)
            throw new InvalidOperationException("Password reset token has already been revoked.");

        if (UsedAtUtc.HasValue)
            throw new InvalidOperationException("Used password reset token cannot be revoked.");

        RevokedAtUtc = DateTime.UtcNow;
        SetUpdated();
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= ExpiresAtUtc;
    }

    public bool IsUsable()
    {
        return !UsedAtUtc.HasValue && !RevokedAtUtc.HasValue && !IsExpired();
    }
}
