using CRM.Application.CRM.Deals.Interfaces;
using CRM.Domain.CRM.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Infrastructure.Repositories.CRMCore.Deals
{
    public sealed class DealRepository : IDealRepository
    {

        private readonly AppDbContext _context;

        public DealRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Deal deal, CancellationToken cancellationToken=default)
        {
            await _context.Deals.AddAsync(deal, cancellationToken);
        }

        public async Task<Deal?> GetByIdAsync(Guid tenandId,Guid dealId,CancellationToken cancellationToken = default)
        {
            return await _context.Deals.FirstOrDefaultAsync(
                x => x.Id == dealId &&
                x.TenantId == tenandId,
                cancellationToken
                );
        }

        public async Task<Deal?> GetDeletedByIdAsync(Guid tenantId,Guid dealId,CancellationToken cancellationToken)
        {
            return await _context.Deals
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                x => x.Id == dealId &&
                x.TenantId == tenantId &&
                x.IsDeleted,
                cancellationToken
                );
        }


    }

}
