using MediatR;

namespace CRM.Application.Identity.Commands.LogoutAll
{
    public class LogoutAllCommand : IRequest
    {
        public Guid TenantId { get; init; }
        public Guid UserId { get; init; }
        public string? DeviceId { get; init; }
        public string? IpAddress { get; init; }
    }
}
