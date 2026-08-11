using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Leads.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.AssignLead;

public sealed class AssignLeadHandler
    : IRequestHandler<AssignLeadCommand>
{
    private readonly ILeadRepository _leadRepository;
    private readonly IUserAssignmentValidator _userAssignmentValidator;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public AssignLeadHandler(
        ILeadRepository leadRepository,
        IUserAssignmentValidator userAssignmentValidator,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _leadRepository = leadRepository;
        _userAssignmentValidator = userAssignmentValidator;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        AssignLeadCommand request,
        CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.LeadId,
            cancellationToken);

        if (lead is null)
        {
            throw new NotFoundException("Lead not found.");
        }

        if (request.OwnerUserId.HasValue)
        {
            var canAssign = await _userAssignmentValidator.CanAssignAsync(
                _currentUser.TenantId,
                request.OwnerUserId.Value,
                cancellationToken);

            if (!canAssign)
            {
                throw new InvalidOperationException(
                    "The selected user cannot be assigned to this lead.");
            }
        }

        lead.AssignOwner(request.OwnerUserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}