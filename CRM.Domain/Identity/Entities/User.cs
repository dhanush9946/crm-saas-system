using CRM.Domain.Common;
using CRM.Domain.Identity.Enums;

namespace CRM.Domain.Identity.Entities
{
    public class User:BaseEntity
    {
        public Guid TenantId { get; private set; }

        public string Email { get; private set; } = default!;
        public string EmailNormalized { get; private set; } = default!;

        public string? PasswordHash { get; private set; }

        public string? DisplayName { get; private set; }

        public UserStatus Status { get; private set; } = UserStatus.Active;

        public bool IsEmailVerified { get; private set; } = false;

        public DateTime? LastLoginAtUtc { get; private set; }


        private User() { }

        private User(Guid tenantId, string email, string displayName)
        {
            if (tenantId == Guid.Empty)
                throw new ArgumentException("TenantId is required");

            TenantId = tenantId;
            SetEmail(email);
            SetDisplayName(displayName);
            Status = UserStatus.Active;
        }

        public static User Create(Guid tenantId,string email,string displayName)
        {
            return new User(tenantId, email, displayName);
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty");

            Email = email.Trim();
            EmailNormalized = email.Trim().ToUpperInvariant();
            SetUpdated();
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty");

            PasswordHash = passwordHash;
            SetUpdated();
        }

        public void SetDisplayName(string displayName)
        {
            DisplayName = displayName?.Trim();
            SetUpdated();
        }

        public void MarkEmailVerified()
        {
            IsEmailVerified = true;
            SetUpdated();
        }

        public void RecordLogin()
        {
            LastLoginAtUtc = DateTime.UtcNow;
            SetUpdated();
        }

        public void Activate()
        {
            Status = UserStatus.Active;
            SetUpdated();
        }

        public void Disable()
        {
            Status = UserStatus.Disabled;
            SetUpdated();
        }
    }
}

