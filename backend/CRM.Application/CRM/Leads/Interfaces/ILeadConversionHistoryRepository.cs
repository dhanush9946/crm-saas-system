using CRM.Domain.CRM.Entities;

namespace CRM.Application.CRM.Leads.Interfaces;

public interface ILeadConversionHistoryRepository
{
    Task AddAsync(
        LeadConversionHistory history,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<LeadConversionHistory>> GetByLeadIdAsync(
        Guid tenantId,
        Guid leadId,
        CancellationToken cancellationToken);
}