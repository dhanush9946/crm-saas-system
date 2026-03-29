

namespace CRM.Application.Identity.Commands.Login
{
    public class LoginCommand
    {
        public string TenantSlug { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? DeviceId { get; set; }
    }
}
