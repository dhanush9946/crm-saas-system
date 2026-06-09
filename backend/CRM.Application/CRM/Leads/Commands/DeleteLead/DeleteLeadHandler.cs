using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Leads.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.DeleteLead;

public sealed class DeleteLeadHandler
    : IRequestHandler<DeleteLeadCommand>
{
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLeadHandler(
        ILeadRepository leadRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _leadRepository = leadRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteLeadCommand request,
        CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.LeadId,
            cancellationToken);

        if (lead is null)
        {
            throw new NotFoundException(
                $"Lead '{request.LeadId}' was not found.");
        }

        lead.SoftDelete();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
