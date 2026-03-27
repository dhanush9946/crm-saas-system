using CRM.Domain.Common;

namespace CRM.Domain.Identity.Entities
{
    public class Role:BaseEntity
    {
        public Guid TenantId { get; private set; }

        public string Name { get; private set; } = default!;
        public string NameNormalized { get; private set; } = default!;

        public bool IsSystemRole { get; private set; } = false;

        private Role() { }

        public Role(Guid tenantId, string name, bool isSystemRole = false)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("TenantId is required");

            TenantId = tenantId;
            SetName(name);
            IsSystemRole = isSystemRole;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty");

            Name = name.Trim();
            NameNormalized = name.Trim().ToUpperInvariant();
            SetUpdated();
        }
    }
}
