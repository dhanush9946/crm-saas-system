

using CRM.Application.Identity.DTOs.Auth;
using MediatR;

namespace CRM.Application.Identity.Commands.RefreshTokenFolder
{
    public class RefreshTokenCommand:IRequest<AuthResponseDto>
    {
        public string RefreshToken { get; set; } = default!;
        public string? DeviceId { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
    }
}
