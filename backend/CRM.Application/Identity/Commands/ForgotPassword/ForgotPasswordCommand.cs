using MediatR;

namespace CRM.Application.Identity.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(
    string TenantSlug,
    string Email,
    string? IpAddress = null,
    string? UserAgent = null,
    string? DeviceId = null,
    string? TraceId = null) : IRequest;
