
using CRM.Application.Identity.DTOs.Auth;

namespace CRM.Application.Identity.Commands.RegisterUser
{
    public interface IRegisterUserHandler
    {
        Task<AuthResponseDto> HandleAsync(RegisterUserCommand command);
    }
}
