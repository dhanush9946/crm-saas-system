

using CRM.Application.Identity.DTOs.Auth;

namespace CRM.Application.Identity.Commands.Login
{
    public interface ILoginHandler
    {
        Task<AuthResponseDto> HandleAsync(LoginCommand command);
    }
}
