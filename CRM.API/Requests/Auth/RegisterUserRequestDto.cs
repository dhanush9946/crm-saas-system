namespace CRM.API.Requests.Auth
{
    public class RegisterUserRequestDto
    {
        public string TenantName { get; set; } = null!;

        public string TenantSlug { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string DisplayName { get; set; } = null!;

        public string? DeviceId { get; set; }
    }
}
