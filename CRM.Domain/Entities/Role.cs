


using CRM.Domain.Common;

namespace CRM.Domain.Entities
{
    public class Role:BaseEntity
    {
        public Guid TenantId { get; private set; }

        public string Name { get; private set; } = null!;
        public string NameNormalized { get; private set; } = null!;

        public bool IsSystemRole { get; private set; }

        private Role() { }

        public Role(Guid tenantId,string name,bool isSystemRole=false)
        {
            Id = Guid.NewGuid();
            TenantId = tenantId;

            Name = name;
            NameNormalized = name.ToUpper();

            IsSystemRole = isSystemRole;
            CreatedAtUtc = DateTime.UtcNow;

        }
    }
}
