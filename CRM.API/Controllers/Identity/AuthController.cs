using CRM.Application.Identity.Commands.Login;
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

        public AuthController(
            IRegisterUserHandler registerHandler,
            ILoginHandler loginHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
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
    }
}
