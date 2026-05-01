using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories.Identity
{
    public class TenantRepository : Repository<Tenant>, ITenantRepository
    {
        public TenantRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Tenant?> GetBySlugAsync(string slug,CancellationToken cancellationToken)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(x => x.Slug == slug,cancellationToken);
        }
    }
}
