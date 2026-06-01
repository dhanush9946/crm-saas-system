using MediatR;

namespace CRM.Application.Identity.Commands.ResendVerificationEmail;

public sealed record ResendVerificationEmailCommand(
    string Email,
    string? IpAddress = null,
    string? UserAgent = null,
    string? DeviceId = null,
    string? TraceId = null) : IRequest;
