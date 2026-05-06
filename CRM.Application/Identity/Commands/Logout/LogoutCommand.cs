using MediatR;

namespace CRM.Application.Identity.Commands.Logout
{
    public class LogoutCommand : IRequest
    {
        public string RefreshToken { get; set; } = default!;
        public string? DeviceId { get; set; }
        public string? IpAddress { get; set; }
    }
}
