using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Activities.Interfaces;
using CRM.Application.CRM.Customers.Interfaces;
using CRM.Application.CRM.Deals.Interfaces;
using CRM.Application.CRM.Leads.Interfaces;
using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;
using MediatR;

namespace CRM.Application.CRM.Activities.Commands.CreateActivity;

public sealed class CreateActivityHandler
    : IRequestHandler<CreateActivityCommand, Guid>
{
    private readonly IActivityRepository _activityRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly IDealRepository _dealRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateActivityHandler(
        IActivityRepository activityRepository,
        ICustomerRepository customerRepository,
        ILeadRepository leadRepository,
        IDealRepository dealRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _activityRepository = activityRepository;
        _customerRepository = customerRepository;
        _leadRepository = leadRepository;
        _dealRepository = dealRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateActivityCommand request,
        CancellationToken cancellationToken)
    {
        await ValidateRelatedEntityAsync(
            request.RelatedEntityType,
            request.RelatedEntityId,
            cancellationToken);

        var activity = Activity.Create(
            tenantId: _currentUser.TenantId,
            type: request.Type,
            subject: request.Subject,
            notes: request.Notes,
            occurredAtUtc: request.OccurredAtUtc,
            dueAtUtc: request.DueAtUtc,
            relatedEntityType: request.RelatedEntityType,
            relatedEntityId: request.RelatedEntityId,
            createdByUserId: _currentUser.UserId);

        await _activityRepository.AddAsync(
            activity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return activity.Id;
    }

    private async Task ValidateRelatedEntityAsync(
        RelatedEntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken)
    {
        switch (entityType)
        {
            case RelatedEntityType.Customer:

                var customer =
                    await _customerRepository.GetByIdAsync(
                        _currentUser.TenantId,
                        entityId,
                        cancellationToken);

                if (customer is null)
                {
                    throw new NotFoundException(
                        $"Customer '{entityId}' was not found.");
                }

                break;

            case RelatedEntityType.Lead:

                var lead =
                    await _leadRepository.GetByIdAsync(
                        _currentUser.TenantId,
                        entityId,
                        cancellationToken);

                if (lead is null)
                {
                    throw new NotFoundException(
                        $"Lead '{entityId}' was not found.");
                }

                break;

            case RelatedEntityType.Deal:

                var deal =
                    await _dealRepository.GetByIdAsync(
                        _currentUser.TenantId,
                        entityId,
                        cancellationToken);

                if (deal is null)
                {
                    throw new NotFoundException(
                        $"Deal '{entityId}' was not found.");
                }

                break;

            default:
                throw new ArgumentException(
                    "Invalid related entity type.");
        }
    }
}