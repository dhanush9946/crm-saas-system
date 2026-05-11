using CRM.API.Responses;
using CRM.Application.Identity.Commands.Login;
using CRM.Application.Identity.Commands.Logout;
using CRM.Application.Identity.Commands.LogoutAll;
using CRM.Application.Identity.Commands.RefreshTokenFolder;
using CRM.Application.Identity.Commands.RegisterUser;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Queries.GetSessions;
using CRM.API.Requests.Auth;
using CRM.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.Application.Identity.Commands.RevokeSession;

namespace CRM.API.Controllers.Identity
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly ICurrentUser _currentUser;

        public AuthController(
               IMediator mediator,
               ICurrentUser currentUser
               )
        {
            _mediatr = mediator;
            _currentUser = currentUser;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequestDto request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand
            {
                TenantName = request.TenantName,
                TenantSlug = request.TenantSlug,
                Email = request.Email,
                Password = request.Password,
                DisplayName = request.DisplayName,
                DeviceId = request.DeviceId,
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            var result = await _mediatr.Send(command, cancellationToken);

            var traceId = HttpContext.TraceIdentifier;

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result,traceId));
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand
            {
                TenantSlug = request.TenantSlug,
                Email = request.Email,
                Password = request.Password,
                DeviceId = request.DeviceId,
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            var result = await _mediatr.Send(command, cancellationToken);

            var traceId = HttpContext.TraceIdentifier;

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result,traceId));
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            var command = new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken,
                DeviceId = request.DeviceId,
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            var result = await _mediatr.Send(command, cancellationToken);

            var traceId = HttpContext.TraceIdentifier;

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result,traceId));
        }

        [Authorize]
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions(CancellationToken cancellationToken)
        {
            var query = new GetSessionsQuery
            {
                UserId = _currentUser.UserId,
                TenantId = _currentUser.TenantId,
                DeviceId = Request.Headers["X-Device-Id"].FirstOrDefault()
            };

            var result = await _mediatr.Send(query, cancellationToken);

            var traceId = HttpContext.TraceIdentifier;

            return Ok(ApiResponse<List<SessionDto>>.SuccessResponse(result, traceId));
        }

        [Authorize]
        [HttpDelete("sessions/{sessionId:guid}")]
        public async Task<IActionResult> RevokeSession(
            Guid sessionId,
            CancellationToken cancellationToken)
        {
            await _mediatr.Send(
                new RevokeSessionCommand(sessionId),
                cancellationToken);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequestDto request, CancellationToken cancellationToken)
        {
            var command = new LogoutCommand
            {
                RefreshToken = request.RefreshToken,
                DeviceId = request.DeviceId,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            await _mediatr.Send(command, cancellationToken);

            return NoContent();
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll(LogoutAllRequestDto? request, CancellationToken cancellationToken)
        {
            var command = new LogoutAllCommand
            {
                UserId = _currentUser.UserId,
                TenantId = _currentUser.TenantId,
                DeviceId = request?.DeviceId ?? Request.Headers["X-Device-Id"].FirstOrDefault(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            await _mediatr.Send(command, cancellationToken);

            return NoContent();
        }

    }
}
