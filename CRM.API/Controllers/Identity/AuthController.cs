using CRM.Application.Identity.Commands.Login;
using CRM.Application.Identity.Commands.RefreshToken;
using CRM.Application.Identity.Commands.RefreshTokenFloder;
using CRM.Application.Identity.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.Identity
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController:ControllerBase
    {
        private readonly IMediator _mediatr;

        private readonly IRegisterUserHandler _registerHandler;
        private readonly IRefreshTokenHandler _refreshHandler;

        public AuthController(
            IRegisterUserHandler registerHandler,
            IMediator mediator,
            IRefreshTokenHandler refreshHandler)
        {
            _registerHandler = registerHandler;
            _mediatr=mediator;
            _refreshHandler = refreshHandler;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            var result = await _registerHandler.HandleAsync(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _mediatr.Send(command);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenCommand command)
        {
            var result = await _refreshHandler.HandleAsync(command);
            return Ok(result);
        }
    }
}
