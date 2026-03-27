

namespace CRM.Application.Identity.DTOs.Auth
{
    public class AuthResponseDto
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}

