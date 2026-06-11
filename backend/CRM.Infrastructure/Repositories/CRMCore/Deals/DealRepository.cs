using CRM.Application.CRM.Deals.Interfaces;
using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Deal?> GetByIdAsync(Guid tenantId,Guid dealId,CancellationToken cancellationToken = default)
        {
            return await _context.Deals.FirstOrDefaultAsync(
                x => x.Id == dealId &&
                x.TenantId == tenantId,
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


        public async Task<bool> ExistsForLeadAndTitleAsync(
    Guid tenantId,
    Guid leadId,
    string title,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return false;
            }

            title = title.Trim();

            return await _context.Deals
                .AsNoTracking()
                .AnyAsync(
                    x => x.TenantId == tenantId &&
                         x.LeadId == leadId &&
                         !x.IsDeleted &&
                         x.Title == title,
                    cancellationToken);
        }


        public async Task<(IReadOnlyList<Deal> Deals, int TotalCount)>
        GetPagedAsync(
            Guid tenantId,
            string? search,
            DealStage? stage,
            Guid? ownerUserId,
            Guid? customerId,
            DateOnly? expectedCloseFrom,
            DateOnly? expectedCloseTo,
            string? sortBy,
            string? sortDirection,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            IQueryable<Deal> query = _context.Deals
                .AsNoTracking()
                .Where(x =>
                    x.TenantId == tenantId &&
                    !x.IsDeleted);

            //---------------------------------
            // Search
            //---------------------------------

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    EF.Functions.Like(
                        x.Title,
                        $"%{search}%"));
            }

            //---------------------------------
            // Filters
            //---------------------------------

            if (stage.HasValue)
            {
                query = query.Where(
                    x => x.Stage == stage.Value);
            }

            if (ownerUserId.HasValue)
            {
                query = query.Where(
                    x => x.OwnerUserId == ownerUserId.Value);
            }

            if (customerId.HasValue)
            {
                query = query.Where(
                    x => x.CustomerId == customerId.Value);
            }

            if (expectedCloseFrom.HasValue)
            {
                query = query.Where(
                    x => x.ExpectedCloseDate >= expectedCloseFrom.Value);
            }

            if (expectedCloseTo.HasValue)
            {
                query = query.Where(
                    x => x.ExpectedCloseDate <= expectedCloseTo.Value);
            }

            //---------------------------------
            // Sorting
            //---------------------------------

            var descending =
                string.Equals(
                    sortDirection,
                    "desc",
                    StringComparison.OrdinalIgnoreCase);

            query = sortBy?.ToLower() switch
            {
                "title" => descending
                    ? query.OrderByDescending(x => x.Title)
                    : query.OrderBy(x => x.Title),

                "value" => descending
                    ? query.OrderByDescending(x => x.Value)
                    : query.OrderBy(x => x.Value),

                "probability" => descending
                    ? query.OrderByDescending(x => x.Probability)
                    : query.OrderBy(x => x.Probability),

                "expectedclosedate" => descending
                    ? query.OrderByDescending(x => x.ExpectedCloseDate)
                    : query.OrderBy(x => x.ExpectedCloseDate),

                "createdatutc" => descending
                    ? query.OrderByDescending(x => x.CreatedAtUtc)
                    : query.OrderBy(x => x.CreatedAtUtc),

                _ => query.OrderByDescending(x => x.CreatedAtUtc)
            };

            //---------------------------------
            // Count
            //---------------------------------

            var totalCount =
                await query.CountAsync(
                    cancellationToken);

            //---------------------------------
            // Paging
            //---------------------------------

            var deals = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (deals, totalCount);
        }

    }

}
