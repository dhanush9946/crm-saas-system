using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Activities.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Activities.Commands.UpdateActivity;

public sealed class UpdateActivityHandler
    : IRequestHandler<UpdateActivityCommand>
{
    private readonly IActivityRepository _activityRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConcurrencyService _concurrencyService;

    public UpdateActivityHandler(
        IActivityRepository activityRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        IConcurrencyService concurrencyService)
    {
        _activityRepository = activityRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _concurrencyService = concurrencyService;
    }

    public async Task Handle(
        UpdateActivityCommand request,
        CancellationToken cancellationToken)
    {
        //-----------------------------------------
        // Get Activity
        //-----------------------------------------

        var activity = await _activityRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.ActivityId,
            cancellationToken);

        if (activity is null)
        {
            throw new NotFoundException(
                $"Activity '{request.ActivityId}' was not found.");
        }

        //-----------------------------------------
        // Apply Original RowVersion
        //-----------------------------------------

        var rowVersion =
            Convert.FromBase64String(
                request.RowVersion);

        _concurrencyService.SetOriginalRowVersion(
            activity,
            rowVersion);

        //-----------------------------------------
        // Domain Update
        //-----------------------------------------

        activity.Update(
            request.Type,
            request.Subject,
            request.Notes,
            request.OccurredAtUtc,
            request.DueAtUtc);

        //-----------------------------------------
        // Save
        //-----------------------------------------

        try
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (ConcurrencyException)
        {
            throw new ConcurrencyException(
                "The activity was modified by another user. Please refresh and try again.");
        }
    }
}