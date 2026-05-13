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

        public int TokenVersion { get; private set; } = 1;

        public DateTime? LastLoginAtUtc { get; private set; }

        public int FailedLoginAttempts { get; private set; }
        public DateTime? LockoutEndUtc { get; private set; }

        public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();


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
            ResetFailedLogins();
            SetUpdated();
        }

        public void RecordFailedLogin()
        {
            FailedLoginAttempts++;
            if (FailedLoginAttempts >= 5)
            {
                LockoutEndUtc = DateTime.UtcNow.AddMinutes(15);
            }
            SetUpdated();
        }

        public void ResetFailedLogins()
        {
            if (FailedLoginAttempts > 0 || LockoutEndUtc.HasValue)
            {
                FailedLoginAttempts = 0;
                LockoutEndUtc = null;
                SetUpdated();
            }
        }

        public bool IsLockedOut()
        {
            return LockoutEndUtc.HasValue && LockoutEndUtc.Value > DateTime.UtcNow;
        }

        public void Activate()
        {
            Status = UserStatus.Active;
            SetUpdated();
        }

        public void Disable()
        {
            Status = UserStatus.Disabled;
            IncrementTokenVersion();
        }

        public void IncrementTokenVersion()
        {
            TokenVersion++;
            SetUpdated();
        }

        public bool IsDisabled()
        {
            return Status == UserStatus.Disabled; 
        }
    }
}

