using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Leads.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.ChangeLeadStatus;

public sealed class ChangeLeadStatusHandler
    : IRequestHandler<ChangeLeadStatusCommand>
{
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeLeadStatusHandler(
        ILeadRepository leadRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _leadRepository = leadRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ChangeLeadStatusCommand request,
        CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.LeadId,
            cancellationToken);

        if (lead is null)
        {
            throw new InvalidOperationException("Lead not found.");
        }

        lead.ChangeStatus(request.Status);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}