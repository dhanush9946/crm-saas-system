using CRM.Domain.Common;
using CRM.Domain.Identity.Entities;

namespace CRM.Domain.Identity.Entities;

public sealed class EmailVerificationToken : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }

    public byte[] TokenHash { get; private set; } = default!;

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime? UsedAtUtc { get; private set; }

    // Navigation
    public User User { get; private set; } = default!;

    private EmailVerificationToken() { }

    private EmailVerificationToken(Guid tenantId, Guid userId, byte[] tokenHash, DateTime expiresAtUtc)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId is required");

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required");

        if (tokenHash == null || tokenHash.Length == 0)
            throw new ArgumentException("TokenHash is required");

        TenantId = tenantId;
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static EmailVerificationToken Create(Guid tenantId, Guid userId, byte[] tokenHash, DateTime expiresAtUtc)
    {
        return new EmailVerificationToken(tenantId, userId, tokenHash, expiresAtUtc);
    }

    public void MarkAsUsed()
    {
        UsedAtUtc = DateTime.UtcNow;
        SetUpdated();
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsUsed() => UsedAtUtc.HasValue;
    public bool IsValid() => !IsUsed() && !IsExpired();
}