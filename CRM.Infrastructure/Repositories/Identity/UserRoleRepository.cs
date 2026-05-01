using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(Guid tenantId, Guid userId, Guid roleId)
    {
        return await _context.UserRoles
            .AnyAsync(ur =>
                ur.TenantId == tenantId &&
                ur.UserId == userId &&
                ur.RoleId == roleId);
    }

    public async Task<List<string>> GetUserRolesAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.TenantId == tenantId && ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);
    }
}