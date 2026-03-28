using CRM.Domain.Common;

namespace CRM.Domain.Identity.Entities
{
    public class RefreshToken:BaseEntity
    {
        public Guid TenantId { get; private set; }

        public Guid UserId { get; private set; }

        public byte[] TokenHash { get; private set; } = default!;

        public Guid TokenFamilyId { get; private set; }

        public DateTime IssuedAtUtc { get; private set; }

        public DateTime ExpiresAtUtc { get; private set; }

        public DateTime? RevokedAtUtc { get; private set; }

        public Guid? ReplacedByTokenId { get; private set; }

        public string? DeviceId { get; private set; }

        public string? UserAgent { get; private set; }

        public string? IpAddress { get; private set; }

        private RefreshToken() { }

        private RefreshToken(
            Guid tenantId,
            Guid userId,
            byte[] tokenHash,
            Guid tokenFamilyId,
            DateTime expiresAtUtc,
            string? deviceId = null,
            string? userAgent = null,
            string? ipAddress = null)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("TenantId is required");

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            TenantId = tenantId;
            UserId = userId;
            TokenHash = tokenHash;
            TokenFamilyId = tokenFamilyId;

            IssuedAtUtc = DateTime.UtcNow;
            ExpiresAtUtc = expiresAtUtc;

            DeviceId = deviceId;
            UserAgent = userAgent;
            IpAddress = ipAddress;
        }

        public static RefreshToken Create(
                    Guid tenantId,
                    Guid userId,
                    byte[] tokenHash,
                    DateTime expiresAtUtc,
                    string? deviceId = null,
                    string? userAgent = null,
                    string? ipAddress = null)
        {
            return new RefreshToken(
                tenantId,
                userId,
                tokenHash,
                Guid.NewGuid(), // TokenFamilyId generated here
                expiresAtUtc,
                deviceId,
                userAgent,
                ipAddress
            );
        }

        public void Revoke(Guid? replacedByTokenId = null)
        {
            RevokedAtUtc = DateTime.UtcNow;
            ReplacedByTokenId = replacedByTokenId;
            SetUpdated();
        }

        public void MarkAsReplaced(Guid newTokenId)
        {
            Revoke(newTokenId);
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow >= ExpiresAtUtc;
        }

        public bool IsActive()
        {
            return RevokedAtUtc == null && !IsExpired();
        }

        public bool IsRevoked()
        {
            return RevokedAtUtc != null;
        }

    }
}
