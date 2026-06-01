using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(Guid tenantId, string normalizedName)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r =>
                r.TenantId == tenantId &&
                r.NameNormalized == normalizedName);
    }
}