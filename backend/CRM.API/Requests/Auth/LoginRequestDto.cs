namespace CRM.API.Requests.Auth
{
    public class LoginRequestDto
    {
        public string TenantSlug { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
