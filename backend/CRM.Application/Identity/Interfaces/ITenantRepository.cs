

using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<Tenant?> GetBySlugAsync(string slug,CancellationToken cancellationToken);
    }
}
