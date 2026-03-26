using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Domain.Entities
{
    public class UserRole
    {
        public Guid UserRoleId { get; private set; }

        public Guid TenantId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        private UserRole() { }

        public UserRole(Guid tenantId, Guid userId, Guid roleId)
        {
            UserRoleId = Guid.NewGuid();
            TenantId = tenantId;
            UserId = userId;
            RoleId = roleId;

            CreatedAtUtc = DateTime.UtcNow;
        }
    }
}
