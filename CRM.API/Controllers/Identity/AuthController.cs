using CRM.Application.Identity.Commands.Login;
using CRM.Application.Identity.Commands.RefreshTokenFolder;
using CRM.Application.Identity.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.Identity
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediatr;
        public AuthController(
               IMediator mediator
               )
        {
            _mediatr = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
