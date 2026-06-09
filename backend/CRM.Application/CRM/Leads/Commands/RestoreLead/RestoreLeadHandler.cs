using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Leads.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.RestoreLead;

public sealed class RestoreLeadHandler
    : IRequestHandler<RestoreLeadCommand>
{
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RestoreLeadHandler(
        ILeadRepository leadRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _leadRepository = leadRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RestoreLeadCommand request,
        CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetDeletedByIdAsync(
            _currentUser.TenantId,
            request.LeadId,
            cancellationToken);

        if (lead is null)
        {
            throw new NotFoundException(
                $"Lead '{request.LeadId}' was not found.");
        }

        if (!string.IsNullOrWhiteSpace(lead.Email))
        {
            var duplicateExists =
                await _leadRepository.EmailExistsAsync(
                    _currentUser.TenantId,
                    lead.Email,
                    cancellationToken);

            if (duplicateExists)
            {
                throw new ConflictException(
                    "Active lead with the same email already exists.");
            }
        }

        lead.Restore();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
