namespace CRM.API.Requests.Auth
{
    public class CompleteGoogleOnboardingRequestDto
    {
        public string IdToken { get; set; } = null!;

        public string TenantName { get; set; } = null!;

        public string TenantSlug { get; set; } = null!;
    }
}
