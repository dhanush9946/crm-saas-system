

namespace CRM.Domain.Entities
{
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; private set; }

        public Guid TenantId { get; private set; }
        public Guid UserId { get; private set; }

        public byte[] TokenHash { get; private set; } = null!;

        public Guid TokenFamilyId { get; private set; }

        public DateTime IssuedAtUtc { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }

        public DateTime? RevokedAtUtc { get; private set; }

        public string DeviceId { get; private set; } = null!;

        private RefreshToken() { }

        public RefreshToken(Guid tenantId, Guid userId, byte[] tokenHash)
        {
            RefreshTokenId = Guid.NewGuid();
            TenantId = tenantId;
            UserId = userId;
            TokenHash = tokenHash;

            TokenFamilyId = Guid.NewGuid();
            IssuedAtUtc = DateTime.UtcNow;
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7);
        }

        public void Revoke()
        {
            RevokedAtUtc = DateTime.UtcNow;
        }
    }
}
