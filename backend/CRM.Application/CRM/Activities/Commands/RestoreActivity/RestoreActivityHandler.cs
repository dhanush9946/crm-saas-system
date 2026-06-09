using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Activities.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Activities.Commands.RestoreActivity;

public sealed class RestoreActivityHandler
    : IRequestHandler<RestoreActivityCommand>
{
    private readonly IActivityRepository _activityRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RestoreActivityHandler(
        IActivityRepository activityRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _activityRepository = activityRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RestoreActivityCommand request,
        CancellationToken cancellationToken)
    {
        var activity =
            await _activityRepository.GetDeletedByIdAsync(
                _currentUser.TenantId,
                request.ActivityId,
                cancellationToken);

        if (activity is null)
        {
            throw new NotFoundException(
                $"Deleted activity '{request.ActivityId}' was not found.");
        }

        activity.Restore();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}