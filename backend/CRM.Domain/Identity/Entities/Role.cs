using CRM.Domain.Common;

namespace CRM.Domain.Identity.Entities
{
    public class Role : BaseEntity
    {
        public Guid TenantId { get; private set; }

        public string Name { get; private set; } = default!;
        public string NameNormalized { get; private set; } = default!;

        public bool IsSystemRole { get; private set; }

        public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        private Role() { }

        private Role(Guid tenantId, string name, bool isSystemRole)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("TenantId is required");

            TenantId = tenantId;
            SetName(name);
            IsSystemRole = isSystemRole;
        }

        public static Role Create(Guid tenantId, string name, bool isSystemRole = false)
        {
            return new Role(tenantId, name, isSystemRole);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty");

            Name = name.Trim();
            NameNormalized = name.Trim().ToUpperInvariant();

            SetUpdated();
        }

        public bool IsOwner()
        {
            return NameNormalized == "OWNER";
        }
    }
}