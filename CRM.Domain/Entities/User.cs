using CRM.Domain.Common;

namespace CRM.Domain.Entities
{
    public class User:BaseEntity
    {
        public Guid TenantId { get; private set; }

        public string Email { get; private set; } = null!;
        public string EmailNormalized { get; private set; } = null!;

        public string PasswordHash { get; private set; } = null!;
        public string DisplayName { get; private set; } = null!;

        public string Status { get; private set; } = null!;
        public bool IsEmailVerified { get; private set; }

        public DateTime? LastLoginAtUtc { get; private set; }

        private readonly List<UserRole> _roles = new();
        public IReadOnlyCollection<UserRole> Roles => _roles;

        private User() { }

        public User(Guid tenantId, string email, string displayName)
        {
            Id = Guid.NewGuid();
            TenantId = tenantId;

            Email = email;
            EmailNormalized = email.ToUpper();

            DisplayName = displayName;
            Status = "Active";

            CreatedAtUtc = DateTime.UtcNow;
        }

        public void VerifyEmail()
        {
            IsEmailVerified = true;
        }
    }
}

