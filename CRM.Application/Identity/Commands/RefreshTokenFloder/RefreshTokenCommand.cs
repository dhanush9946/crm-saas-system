

namespace CRM.Application.Identity.Commands.RefreshToken
{
    public class RefreshTokenCommand
    {
        public string RefreshToken { get; set; } = default!;
        public string? DeviceId { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
    }
}
