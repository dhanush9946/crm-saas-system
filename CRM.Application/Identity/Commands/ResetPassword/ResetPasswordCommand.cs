using MediatR;

namespace CRM.Application.Identity.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string TenantSlug,
    string Token,
    string NewPassword,
    string? IpAddress = null,
    string? UserAgent = null,
    string? DeviceId = null,
    string? TraceId = null) : IRequest;
