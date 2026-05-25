namespace CRM.API.Requests.Auth
{
    public class ForgotPasswordRequestDto
    {
        public string TenantSlug { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
