using CRM.API.Helpers;
using CRM.API.Requests.Auth;
using CRM.API.Responses;
using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.Commands.CompleteGoogleOnboarding;
using CRM.Application.Identity.Commands.Login;
using CRM.Application.Identity.Commands.LoginWithGoogle;
using CRM.Application.Identity.Commands.ForgotPassword;
using CRM.Application.Identity.Commands.ResetPassword;
using CRM.Application.Identity.Commands.Logout;
using CRM.Application.Identity.Commands.LogoutAll;
using CRM.Application.Identity.Commands.RefreshTokenFolder;
using CRM.Application.Identity.Commands.RegisterUser;
using CRM.Application.Identity.Commands.ResendVerificationEmail;
using CRM.Application.Identity.Commands.RevokeSession;
using CRM.Application.Identity.Commands.VerifyEmail;
using CRM.Application.Identity.DTOs.Auth;
using CRM.Application.Identity.Queries.GetSessions;
using CRM.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CRM.API.Controllers.Identity
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediatr;
        private readonly ICurrentUser _currentUser;
        private readonly IConfiguration _configuration;

        public AuthController(
               IMediator mediator,
               ICurrentUser currentUser,
               IConfiguration configuration)
        {
            _mediatr = mediator;
            _currentUser = currentUser;
            _configuration = configuration;
        }

        [EnableRateLimiting(RateLimitPolicies.RegisterPolicy)]
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
                DeviceId = DeviceIdHeaderHelper.GetDeviceId(Request),
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                TraceId = HttpContext.TraceIdentifier
            };

            var result = await _mediatr.Send(command, cancellationToken);

            return Ok(WriteAuthResponse(result));
        }

        [EnableRateLimiting(RateLimitPolicies.LoginPolicy)]
        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<IActionResult> LoginWithGoogle(
            LoginWithGoogleRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new LoginWithGoogleCommand
            {
                IdToken = request.IdToken,
                DeviceId = DeviceIdHeaderHelper.GetDeviceId(Request),
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                TraceId = HttpContext.TraceIdentifier
            };

            var result = await _mediatr.Send(command, cancellationToken);

            if (!result.RequiresOnboarding &&
                !string.IsNullOrWhiteSpace(result.RefreshToken))
            {
                var expirationDays = _configuration.GetValue(
                    "Jwt:RefreshTokenExpirationDays",
                    7);

                RefreshTokenCookieHelper.Set(
                    Response,
                    result.RefreshToken,
                    expirationDays,
                    _configuration);
            }

            var publicResult = new LoginWithGoogleResponse
            {
                RequiresOnboarding = result.RequiresOnboarding,
                AccessToken = result.AccessToken,
                UserId = result.UserId,
                TenantId = result.TenantId,
                SessionId = result.SessionId,
                Email = result.Email,
                FullName = result.FullName
            };

            return Ok(ApiResponse<LoginWithGoogleResponse>.SuccessResponse(
                publicResult,
                HttpContext.TraceIdentifier));
        }

        [EnableRateLimiting(RateLimitPolicies.LoginPolicy)]
        [AllowAnonymous]
        [HttpPost("google/onboarding")]
        public async Task<IActionResult> CompleteGoogleOnboarding(
            CompleteGoogleOnboardingRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new CompleteGoogleOnboardingCommand
            {
                IdToken = request.IdToken,
                TenantName = request.TenantName,
                TenantSlug = request.TenantSlug,
                DeviceId = DeviceIdHeaderHelper.GetDeviceId(Request),
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                TraceId = HttpContext.TraceIdentifier
            };

            var result = await _mediatr.Send(command, cancellationToken);

            return Ok(WriteAuthResponse(result));
        }

        [EnableRateLimiting(RateLimitPolicies.LoginPolicy)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand
            {
                TenantSlug = request.TenantSlug,
                Email = request.Email,
                Password = request.Password,
                DeviceId = DeviceIdHeaderHelper.GetDeviceId(Request),
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                TraceId = HttpContext.TraceIdentifier
            };

            var result = await _mediatr.Send(command, cancellationToken);

            return Ok(WriteAuthResponse(result));
        }

        [EnableRateLimiting(RateLimitPolicies.RefreshPolicy)]
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshTokenRequestDto? request,
            CancellationToken cancellationToken)
        {
            var refreshToken = ResolveRefreshToken(request?.RefreshToken);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized();
            }

            var command = new RefreshTokenCommand
            {
                RefreshToken = refreshToken,
                DeviceId = DeviceIdHeaderHelper.GetDeviceId(Request),
                UserAgent = Request.Headers.UserAgent.ToString(),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                TraceId = HttpContext.TraceIdentifier
            };

            var result = await _mediatr.Send(command, cancellationToken);

            return Ok(WriteAuthResponse(result));
        }

        [Authorize]
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions(CancellationToken cancellationToken)
        {
            var query = new GetSessionsQuery
            {
                UserId = _currentUser.UserId,
                TenantId = _currentUser.TenantId,
                SessionId = _currentUser.SessionId,
                DeviceId = DeviceIdHeaderHelper.GetDeviceId(Request)
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
                new RevokeSessionCommand(
                    sessionId,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers.UserAgent.ToString(),
                    HttpContext.TraceIdentifier),
                cancellationToken);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            [FromBody] LogoutRequestDto? request,
            CancellationToken cancellationToken)
        {
            var refreshToken = ResolveRefreshToken(request?.RefreshToken);
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var command = new LogoutCommand
                {
                    RefreshToken = refreshToken,
                    DeviceId = DeviceIdHeaderHelper.GetDeviceId(Request),
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers.UserAgent.ToString(),
                    TraceId = HttpContext.TraceIdentifier
                };

                await _mediatr.Send(command, cancellationToken);
            }

            RefreshTokenCookieHelper.Clear(Response, _configuration);

            return NoContent();
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
        {
            var command = new LogoutAllCommand
            {
                UserId = _currentUser.UserId,
                TenantId = _currentUser.TenantId,
                SessionId = _currentUser.SessionId,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString(),
                TraceId = HttpContext.TraceIdentifier
            };

            await _mediatr.Send(command, cancellationToken);

            RefreshTokenCookieHelper.Clear(Response, _configuration);

            return NoContent();
        }

        [EnableRateLimiting(RateLimitPolicies.LoginPolicy)]
        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(
                 VerifyEmailRequestDto request,
                 CancellationToken cancellationToken)
        {
            var command = new VerifyEmailCommand(
                request.Token,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                DeviceIdHeaderHelper.GetDeviceId(Request),
                HttpContext.TraceIdentifier);

            await _mediatr.Send(
                command,
                cancellationToken);

            return NoContent();
        }

        [AllowAnonymous]
        [EnableRateLimiting(
        RateLimitPolicies.RegisterPolicy)]
        [HttpPost("resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmail(
            ResendVerificationEmailRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new ResendVerificationEmailCommand(
                request.Email,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                DeviceIdHeaderHelper.GetDeviceId(Request),
                HttpContext.TraceIdentifier);

            await _mediatr.Send(
                command,
                cancellationToken);

            return NoContent();
        }

        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.LoginPolicy)]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new ForgotPasswordCommand(
                request.TenantSlug,
                request.Email,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                DeviceIdHeaderHelper.GetDeviceId(Request),
                HttpContext.TraceIdentifier);

            await _mediatr.Send(command, cancellationToken);

            return NoContent();
        }

        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.LoginPolicy)]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new ResetPasswordCommand(
                request.TenantSlug,
                request.Token,
                request.NewPassword,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                DeviceIdHeaderHelper.GetDeviceId(Request),
                HttpContext.TraceIdentifier);

            await _mediatr.Send(command, cancellationToken);

            return NoContent();
        }

        private string? ResolveRefreshToken(string? bodyToken) =>
            !string.IsNullOrWhiteSpace(bodyToken)
                ? bodyToken
                : RefreshTokenCookieHelper.Get(Request);

        private ApiResponse<AuthPublicResponseDto> WriteAuthResponse(AuthResponseDto result)
        {
            var expirationDays = _configuration.GetValue("Jwt:RefreshTokenExpirationDays", 7);
            RefreshTokenCookieHelper.Set(Response, result.RefreshToken, expirationDays, _configuration);

            var traceId = HttpContext.TraceIdentifier;
            var publicPayload = new AuthPublicResponseDto
            {
                TenantId = result.TenantId,
                UserId = result.UserId,
                SessionId = result.SessionId,
                AccessToken = result.AccessToken,
            };

            return ApiResponse<AuthPublicResponseDto>.SuccessResponse(publicPayload, traceId);
        }
    }
}
