

namespace CRM.Domain.Entities
{
    public class ExternalLogin
    {
        public Guid ExternalLoginId { get; private set; }

        public Guid TenantId { get; private set; }
        public Guid UserId { get; private set; }

        public string Provider { get; private set; } = null!;
        public string ProviderUserId { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        public DateTime CreatedAtUtc { get; private set; }

        private ExternalLogin() { }

        public ExternalLogin(Guid tenantId, Guid userId, string provider, string providerUserId, string email)
        {
            ExternalLoginId = Guid.NewGuid();
            TenantId = tenantId;
            UserId = userId;

            Provider = provider;
            ProviderUserId = providerUserId;
            Email = email;

            CreatedAtUtc = DateTime.UtcNow;
        }
    }
}
