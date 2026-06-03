using CRM.Application.CRM.Leads.Interfaces;
using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;
using CRM.Domain.Leads.Enums;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public sealed class LeadRepository : ILeadRepository
{
    private readonly AppDbContext _context;

    public LeadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Lead lead,
        CancellationToken cancellationToken = default)
    {
        await _context.Leads.AddAsync(
            lead,
            cancellationToken);
    }

    public async Task<Lead?> GetByIdAsync(
        Guid tenantId,
        Guid leadId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Leads
            .FirstOrDefaultAsync(
                x => x.Id == leadId &&
                     x.TenantId == tenantId,
                     cancellationToken);
    }

    public async Task<Lead?> GetDeletedByIdAsync(
        Guid tenantId,
        Guid leadId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Leads
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Id == leadId &&
                     x.TenantId == tenantId &&
                     x.IsDeleted,
                cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(
        Guid tenantId,
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Leads.AnyAsync(
            x => x.TenantId == tenantId &&
                 x.Email == email,
                 cancellationToken);
    }

    public async Task<bool> EmailExistsForOtherLeadAsync(
        Guid tenantId,
        Guid leadId,
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Leads.AnyAsync(
            x => x.TenantId == tenantId &&
                 x.Id != leadId &&
                 x.Email == email,
            cancellationToken);
    }

    public async Task<(IReadOnlyList<Lead> Leads, int TotalCount)>
        GetPagedAsync(
            Guid tenantId,
            string? search,
            LeadStatus? status,
            LeadSource? source,
            string? sortBy,
            string? sortDirection,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
    {
        IQueryable<Lead> query = _context.Leads
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
                (x.FirstName != null &&
                 EF.Functions.Like(
                     x.FirstName,
                     $"%{search}%"))
                ||
                (x.LastName != null &&
                 EF.Functions.Like(
                     x.LastName,
                     $"%{search}%"))
                ||
                (x.Email != null &&
                 EF.Functions.Like(
                     x.Email,
                     $"%{search}%"))
                ||
                (x.Company != null &&
                 EF.Functions.Like(
                     x.Company,
                     $"%{search}%")));
        }

        //---------------------------------
        // Filters
        //---------------------------------

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        if (source.HasValue)
        {
            query = query.Where(
                x => x.Source == source.Value);
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
            "firstname" => descending
                ? query.OrderByDescending(x => x.FirstName)
                : query.OrderBy(x => x.FirstName),

            "score" => descending
                ? query.OrderByDescending(x => x.Score)
                : query.OrderBy(x => x.Score),

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

        var leads = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (leads, totalCount);
    }
}