using MediatR;

namespace CRM.Application.Identity.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(
    string Token,
    string? IpAddress = null,
    string? UserAgent = null,
    string? DeviceId = null,
    string? TraceId = null) : IRequest;