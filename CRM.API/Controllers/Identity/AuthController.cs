using CRM.Application.Identity.Commands.Login;
using CRM.Application.Identity.Commands.RefreshToken;
using CRM.Application.Identity.Commands.RefreshTokenFloder;
using CRM.Application.Identity.Commands.RegisterUser;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.Identity
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController:ControllerBase
    {
        private readonly IRegisterUserHandler _registerHandler;
        private readonly ILoginHandler _loginHandler;
        private readonly IRefreshTokenHandler _refreshHandler;

        public AuthController(
            IRegisterUserHandler registerHandler,
            ILoginHandler loginHandler,
            IRefreshTokenHandler refreshHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
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
            var result = await _loginHandler.HandleAsync(command);
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
