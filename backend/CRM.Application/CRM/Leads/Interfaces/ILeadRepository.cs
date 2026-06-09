using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;
using CRM.Domain.Leads.Enums;

namespace CRM.Application.CRM.Leads.Interfaces;

public interface ILeadRepository
{
    Task AddAsync(
        Lead lead,
        CancellationToken cancellationToken = default);

    Task<Lead?> GetByIdAsync(
        Guid tenantId,
        Guid leadId,
        CancellationToken cancellationToken = default);

    Task<Lead?> GetDeletedByIdAsync(
        Guid tenantId,
        Guid leadId,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        Guid tenantId,
        string email,
        CancellationToken cancellationToken);

    Task<bool> EmailExistsForOtherLeadAsync(
        Guid tenantId,
        Guid leadId,
        string email,
        CancellationToken cancellationToken);

    Task<(IReadOnlyList<Lead> Leads, int TotalCount)>
        GetPagedAsync(
            Guid tenantId,
            string? search,
            LeadStatus? status,
            LeadSource? source,
            string? sortBy,
            string? sortDirection,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
}