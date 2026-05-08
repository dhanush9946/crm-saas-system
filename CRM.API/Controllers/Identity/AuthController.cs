using CRM.API.Responses;
using CRM.Application.Common.Exceptions;
using CRM.Application.Identity.Commands.Login;
using CRM.Application.Identity.Commands.Logout;
using CRM.Application.Identity.Commands.LogoutAll;
using CRM.Application.Identity.Commands.RefreshTokenFolder;
using CRM.Application.Identity.Commands.RegisterUser;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Queries.GetSessions;
using CRM.API.Requests.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                UserId = GetRequiredGuidClaim(ClaimTypes.NameIdentifier, "sub"),
                TenantId = GetRequiredGuidClaim("tenantId"),
                DeviceId = Request.Headers["X-Device-Id"].FirstOrDefault()
            };

            var result = await _mediatr.Send(query, cancellationToken);

            var traceId = HttpContext.TraceIdentifier;

            return Ok(ApiResponse<List<SessionDto>>.SuccessResponse(result, traceId));
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
                UserId = GetRequiredGuidClaim(ClaimTypes.NameIdentifier, "sub"),
                TenantId = GetRequiredGuidClaim("tenantId"),
                DeviceId = request?.DeviceId ?? Request.Headers["X-Device-Id"].FirstOrDefault(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            await _mediatr.Send(command, cancellationToken);

            return NoContent();
        }

        private Guid GetRequiredGuidClaim(params string[] claimTypes)
        {
            foreach (var claimType in claimTypes)
            {
                var value = User.FindFirstValue(claimType);

                if (Guid.TryParse(value, out var claimValue))
                    return claimValue;
            }

            throw new UnauthorizedException("Invalid authentication token");
        }
    }
}
