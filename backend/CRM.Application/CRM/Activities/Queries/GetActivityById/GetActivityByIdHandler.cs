using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Activities.DTOs;
using CRM.Application.CRM.Activities.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Activities.Queries.GetActivityById;

public sealed class GetActivityByIdHandler
    : IRequestHandler<
        GetActivityByIdQuery,
        ActivityDetailsDto>
{
    private readonly IActivityRepository _activityRepository;
    private readonly ICurrentUser _currentUser;

    public GetActivityByIdHandler(
        IActivityRepository activityRepository,
        ICurrentUser currentUser)
    {
        _activityRepository = activityRepository;
        _currentUser = currentUser;
    }

    public async Task<ActivityDetailsDto> Handle(
        GetActivityByIdQuery request,
        CancellationToken cancellationToken)
    {
        var activity =
            await _activityRepository.GetByIdAsync(
                _currentUser.TenantId,
                request.ActivityId,
                cancellationToken);

        if (activity is null)
        {
            throw new NotFoundException(
                $"Activity '{request.ActivityId}' was not found.");
        }

        return new ActivityDetailsDto
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
            CreatedAtUtc = activity.CreatedAtUtc,
            UpdatedAtUtc = activity.UpdatedAtUtc,
            RowVersion = activity.RowVersion is null
                         ? string.Empty
                         : Convert.ToBase64String(activity.RowVersion)
        };
    }
}