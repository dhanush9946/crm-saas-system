using CRM.Domain.Identity.Entities;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(Guid tenantId, string normalizedName);
}