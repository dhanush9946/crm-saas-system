using MediatR;

namespace CRM.Application.Identity.Commands.LogoutAll
{
    public class LogoutAllCommand : IRequest
    {
        public Guid TenantId { get; init; }
        public Guid UserId { get; init; }
        public Guid? SessionId { get; init; }
        public string? IpAddress { get; init; }
    }
}
