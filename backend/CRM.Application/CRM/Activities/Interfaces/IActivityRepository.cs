using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;

namespace CRM.Application.CRM.Activities.Interfaces;

public interface IActivityRepository
{
    Task AddAsync(
        Activity activity,
        CancellationToken cancellationToken = default);

    Task<Activity?> GetByIdAsync(
        Guid tenantId,
        Guid activityId,
        CancellationToken cancellationToken = default);

    Task<Activity?> GetDeletedByIdAsync(
        Guid tenantId,
        Guid activityId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Activity> Activities, int TotalCount)>
        GetPagedAsync(
            Guid tenantId,
            RelatedEntityType? relatedEntityType,
            Guid? relatedEntityId,
            ActivityType? activityType,
            DateTime? dueFrom,
            DateTime? dueTo,
            string? sortBy,
            string? sortDirection,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
}