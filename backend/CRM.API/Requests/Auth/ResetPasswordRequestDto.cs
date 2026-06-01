namespace CRM.API.Requests.Auth
{
    public class ResetPasswordRequestDto
    {
        public string TenantSlug { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
