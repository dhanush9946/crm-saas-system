

using CRM.Application.CRM.Activities.Interfaces;
using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace CRM.Infrastructure.Repositories.CRMCore.Activities
{
    public class ActivityRepository:IActivityRepository
    {
        private readonly AppDbContext _context;
        public ActivityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            Activity activity,
            CancellationToken cancellationToken=default)
        {
            await _context.Activities.AddAsync(
                activity,
                cancellationToken);
        }

        public async Task<Activity?> GetByIdAsync(
            Guid tenandId,Guid activityId,
            CancellationToken cancellationToken)
        {
            return await _context.Activities.FirstOrDefaultAsync(
                x => x.Id == activityId &&
                x.TenantId == tenandId,
                cancellationToken);
        }

        public async Task<Activity?> GetDeletedByIdAsync(
            Guid tenandId,Guid activityId,CancellationToken cancellationToken)
        {
            return await _context.Activities
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                x => x.Id == activityId &&
                x.TenantId == tenandId &&
                x.IsDeleted,
                cancellationToken);
        }

        public async Task<
    (IReadOnlyList<Activity> Activities,
    int TotalCount)>
    GetPagedAsync(
        Guid tenantId,
        string? search,
        RelatedEntityType? relatedEntityType,
        Guid? relatedEntityId,
        ActivityType? activityType,
        bool? isCompleted,
        DateTime? dueFrom,
        DateTime? dueTo,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
        {
            IQueryable<Activity> query =
                _context.Activities
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
                        x.Subject,
                        $"%{search}%")
                    ||
                    (x.Notes != null &&
                     EF.Functions.Like(
                         x.Notes,
                         $"%{search}%")));
            }

            //---------------------------------
            // Filters
            //---------------------------------

            if (relatedEntityType.HasValue)
            {
                query = query.Where(
                    x => x.RelatedEntityType ==
                         relatedEntityType.Value);
            }

            if (relatedEntityId.HasValue)
            {
                query = query.Where(
                    x => x.RelatedEntityId ==
                         relatedEntityId.Value);
            }

            if (activityType.HasValue)
            {
                query = query.Where(
                    x => x.Type ==
                         activityType.Value);
            }

            if (isCompleted.HasValue)
            {
                query = isCompleted.Value
                    ? query.Where(x => x.CompletedAtUtc != null)
                    : query.Where(x => x.CompletedAtUtc == null);
            }

            if (dueFrom.HasValue)
            {
                query = query.Where(
                    x => x.DueAtUtc >= dueFrom.Value);
            }

            if (dueTo.HasValue)
            {
                query = query.Where(
                    x => x.DueAtUtc <= dueTo.Value);
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
                "subject" => descending
                    ? query.OrderByDescending(x => x.Subject)
                    : query.OrderBy(x => x.Subject),

                "type" => descending
                    ? query.OrderByDescending(x => x.Type)
                    : query.OrderBy(x => x.Type),

                "occurredatutc" => descending
                    ? query.OrderByDescending(x => x.OccurredAtUtc)
                    : query.OrderBy(x => x.OccurredAtUtc),

                "dueatutc" => descending
                    ? query.OrderByDescending(x => x.DueAtUtc)
                    : query.OrderBy(x => x.DueAtUtc),

                "completedatutc" => descending
                    ? query.OrderByDescending(x => x.CompletedAtUtc)
                    : query.OrderBy(x => x.CompletedAtUtc),

                "createdatutc" => descending
                    ? query.OrderByDescending(x => x.CreatedAtUtc)
                    : query.OrderBy(x => x.CreatedAtUtc),

                _ => query.OrderByDescending(
                    x => x.CreatedAtUtc)
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

            var activities = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (activities, totalCount);
        }
    }
}

