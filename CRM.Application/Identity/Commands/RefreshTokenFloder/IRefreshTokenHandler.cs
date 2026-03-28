

using CRM.Application.Identity.Commands.RefreshToken;
using CRM.Application.Identity.DTOs.Auth;

namespace CRM.Application.Identity.Commands.RefreshTokenFloder
{
    public interface IRefreshTokenHandler
    {
        Task<AuthResponseDto> HandleAsync(RefreshTokenCommand command);
    }
}
