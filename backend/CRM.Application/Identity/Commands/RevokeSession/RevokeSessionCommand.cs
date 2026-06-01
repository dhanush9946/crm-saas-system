

using MediatR;

namespace CRM.Application.Identity.Commands.RevokeSession;

public sealed record RevokeSessionCommand(
    Guid SessionId,
    string? IpAddress = null,
    string? UserAgent = null,
    string? TraceId = null) : IRequest;
