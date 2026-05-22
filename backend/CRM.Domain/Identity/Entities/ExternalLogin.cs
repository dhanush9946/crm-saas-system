namespace CRM.Domain.Identity.Entities
{
    public class ExternalLogin
    {
        public Guid TenantId { get; private set; }

        public Guid UserId { get; private set; }

        public string Provider { get; private set; } = default!; 

        public string ProviderUserId { get; private set; } = default!; // Google unique ID

        public string? Email { get; private set; }

        // EF Core
        private ExternalLogin() { }

        public ExternalLogin(
            Guid tenantId,
            Guid userId,
            string provider,
            string providerUserId,
            string? email = null)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("TenantId is required");

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("Provider is required");

            if (string.IsNullOrWhiteSpace(providerUserId))
                throw new ArgumentException("ProviderUserId is required");

            TenantId = tenantId;
            UserId = userId;
            Provider = provider.Trim();
            ProviderUserId = providerUserId.Trim();
            Email = email?.Trim();
        }
    }
}
