

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

        public async Task<Tenant?> GetByIdAsync(Guid id)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Tenant?> GetBySlugAsync(string slug)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task AddAsync(Tenant tenant)
        {
            await _context.Tenants.AddAsync(tenant);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
