

using CRM.Application.Identity.Interfaces;
using CRM.Domain.Identity.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories
{
    public class TenantRepository:ITenantRepository
    {
        private readonly AppDbContext _context;

        public TenantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetByIdAsync(Guid id,CancellationToken cancellationToken)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
        }

        public async Task<Tenant?> GetBySlugAsync(string slug,CancellationToken cancellationToken)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(x => x.Slug == slug,cancellationToken);
        }

        public async Task AddAsync(Tenant tenant,CancellationToken cancellationToken)
        {
            await _context.Tenants.AddAsync(tenant,cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
