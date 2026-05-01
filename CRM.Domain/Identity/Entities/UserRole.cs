using CRM.Domain.Common;

namespace CRM.Domain.Identity.Entities
{
    public class UserRole : BaseEntity
    {
        public Guid TenantId { get; private set; }

        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }

        // Optional navigation
        public User User { get; private set; } = default!;
        public Role Role { get; private set; } = default!;

        private UserRole() { }

        private UserRole(Guid tenantId, Guid userId, Guid roleId)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("TenantId is required");

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            if (roleId == Guid.Empty)
                throw new ArgumentException("RoleId is required");

            TenantId = tenantId;
            UserId = userId;
            RoleId = roleId;
        }

        public static UserRole Create(Guid tenantId, Guid userId, Guid roleId)
        {
            return new UserRole(tenantId, userId, roleId);
        }
    }
}