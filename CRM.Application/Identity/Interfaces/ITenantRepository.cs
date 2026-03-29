

using CRM.Domain.Identity.Entities;

namespace CRM.Application.Identity.Interfaces
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetByIdAsync(Guid id);

        Task<Tenant?> GetBySlugAsync(string slug);

        Task AddAsync(Tenant tenant);

        Task SaveChangesAsync();
    }
}
