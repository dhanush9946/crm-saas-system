

using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetByIdAsync(Guid id,CancellationToken cancellationToken);

        Task<Tenant?> GetBySlugAsync(string slug,CancellationToken cancellationToken);

        Task AddAsync(Tenant tenant,CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
