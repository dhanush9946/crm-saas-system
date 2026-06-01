using CRM.Domain.Identity.Entities;

public interface IUserRoleRepository : IRepository<UserRole>
{
    Task<bool> ExistsAsync(Guid tenantId, Guid userId, Guid roleId);
    Task<List<string>> GetUserRolesAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken);
}