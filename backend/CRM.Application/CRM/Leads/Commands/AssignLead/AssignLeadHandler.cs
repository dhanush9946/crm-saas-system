using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Leads.Interfaces;
using CRM.Application.Identity.Interfaces; // Adjust namespace to your IUserRepository
using MediatR;
using CRM.Domain.Identity.Enums;
using CRM.Application.Common.Exceptions;

namespace CRM.Application.CRM.Leads.Commands.AssignLead;

public sealed class AssignLeadHandler
    : IRequestHandler<AssignLeadCommand>
{
    private readonly ILeadRepository _leadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public AssignLeadHandler(
        ILeadRepository leadRepository,
        IUserRepository userRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _leadRepository = leadRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        AssignLeadCommand request,
        CancellationToken cancellationToken)
    {
        // Load the lead for the current tenant
        var lead = await _leadRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.LeadId,
            cancellationToken);

        if (lead is null)
        {
            throw new InvalidOperationException("Lead not found.");
        }

        // Validate the owner if one was provided
        if (request.OwnerUserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(
                _currentUser.TenantId,
                request.OwnerUserId.Value,
                cancellationToken);

            if (user is null)
            {
                throw new NotFoundException(
                    "Assigned user was not found.");
            }

            if (user.Status == UserStatus.Disabled)
            {
                throw new InvalidOperationException(
                    "Cannot assign the lead to a deleted user.");
            }
        }

        // Assign or unassign the owner
        lead.AssignOwner(request.OwnerUserId);

        // Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}