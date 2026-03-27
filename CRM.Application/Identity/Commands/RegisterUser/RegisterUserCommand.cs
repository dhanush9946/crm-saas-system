

namespace CRM.Application.Identity.Commands.RegisterUser
{
    public class RegisterUserCommand
    {
        public string TenantName { get; set; } = null!;
        public string TenantSlug { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }
}
