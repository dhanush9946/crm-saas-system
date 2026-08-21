using CRM.Application.CRM.Leads.Interfaces;
using CRM.Domain.CRM.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public sealed class LeadConversionHistoryRepository
    : ILeadConversionHistoryRepository
{
    private readonly AppDbContext _context;

    public LeadConversionHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        LeadConversionHistory history,
        CancellationToken cancellationToken)
    {
        await _context.LeadConversionHistories.AddAsync(
            history,
            cancellationToken);
    }

    public async Task<IReadOnlyList<LeadConversionHistory>>
        GetByLeadIdAsync(
            Guid tenantId,
            Guid leadId,
            CancellationToken cancellationToken)
    {
        return await _context.LeadConversionHistories
            .Where(x =>
                x.TenantId == tenantId &&
                x.LeadId == leadId)
            .OrderByDescending(x => x.ConvertedAtUtc)
            .ToListAsync(cancellationToken);
    }
}