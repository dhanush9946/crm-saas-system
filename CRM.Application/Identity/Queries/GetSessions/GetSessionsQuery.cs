using CRM.Application.Identity.DTOs.Auth;
using MediatR;

namespace CRM.Application.Identity.Queries.GetSessions
{
    public class GetSessionsQuery : IRequest<List<SessionDto>>
    {
        public Guid TenantId { get; set; }

        public Guid UserId { get; set; }

        public string? DeviceId { get; set; }
    }
}
