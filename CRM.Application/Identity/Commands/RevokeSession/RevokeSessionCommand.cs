

using MediatR;

namespace CRM.Application.Identity.Commands.RevokeSession;

public sealed record RevokeSessionCommand(Guid SessionId): IRequest;