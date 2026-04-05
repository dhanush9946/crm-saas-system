

using CRM.Application.Identity.DTOs.Auth;
using MediatR;

namespace CRM.Application.Identity.Commands.RegisterUser
{
    public class RegisterUserCommand:IRequest<AuthResponseDto>
    {
        public string TenantName { get; set; } = null!;
        public string TenantSlug { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }
}
