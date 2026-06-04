
using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;

namespace CRM.Application.CRM.Deals.Interfaces;

public interface IDealRepository
{
    Task AddAsync(
        Deal deal,
        CancellationToken cancellationToken = default);

    Task<Deal?> GetByIdAsync(
        Guid tenantId,
        Guid dealId,
        CancellationToken cancellationToken = default);

    Task<Deal?> GetDeletedByIdAsync(
        Guid tenantId,
        Guid dealId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Deal> Deals, int TotalCount)>
        GetPagedAsync(
            Guid tenantId,
            DealStage? stage,
            Guid? ownerUserId,
            DateOnly? expectedCloseFrom,
            DateOnly? expectedCloseTo,
            string? sortBy,
            string? sortDirection,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
}
