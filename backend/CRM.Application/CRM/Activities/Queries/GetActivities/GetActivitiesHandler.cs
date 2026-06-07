using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Activities.DTOs;
using CRM.Application.CRM.Activities.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Activities.Queries.GetActivities;

public sealed class GetActivitiesHandler
    : IRequestHandler<
        GetActivitiesQuery,
        PagedResult<ActivityDto>>
{
    private readonly IActivityRepository _activityRepository;
    private readonly ICurrentUser _currentUser;

    public GetActivitiesHandler(
        IActivityRepository activityRepository,
        ICurrentUser currentUser)
    {
        _activityRepository = activityRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<ActivityDto>> Handle(
        GetActivitiesQuery request,
        CancellationToken cancellationToken)
    {
        var (activities, totalCount) =
            await _activityRepository.GetPagedAsync(
                _currentUser.TenantId,
                request.Search,
                request.RelatedEntityType,
                request.RelatedEntityId,
                request.ActivityType,
                request.IsCompleted,
                request.DueFrom,
                request.DueTo,
                request.SortBy,
                request.SortDirection,
                request.Page,
                request.PageSize,
                cancellationToken);

        var items = activities
            .Select(activity => new ActivityDto
            {
                Id = activity.Id,
                Type = activity.Type,
                Subject = activity.Subject,
                Notes = activity.Notes,
                OccurredAtUtc = activity.OccurredAtUtc,
                DueAtUtc = activity.DueAtUtc,
                CompletedAtUtc = activity.CompletedAtUtc,
                RelatedEntityType = activity.RelatedEntityType,
                RelatedEntityId = activity.RelatedEntityId,
                CreatedByUserId = activity.CreatedByUserId,
                CreatedAtUtc = activity.CreatedAtUtc              
            })
            .ToList();

        return new PagedResult<ActivityDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}