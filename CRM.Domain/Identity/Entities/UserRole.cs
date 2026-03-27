

namespace CRM.Domain.Identity.Entities
{
    public class UserRole
    {
        public Guid TenantId { get; private set; }

        public Guid UserId { get; private set; }

        public Guid RoleId { get; private set; }

        
        private UserRole() { }

        public UserRole(Guid tenantId, Guid userId, Guid roleId)
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
    }
}
