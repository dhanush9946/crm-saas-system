using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Leads.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.UpdateLead;

public sealed class UpdateLeadHandler
    : IRequestHandler<UpdateLeadCommand>
{
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConcurrencyService _concurrencyService;

    public UpdateLeadHandler(
        ILeadRepository leadRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        IConcurrencyService concurrencyService)
    {
        _leadRepository = leadRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _concurrencyService = concurrencyService;
    }

    public async Task Handle(
        UpdateLeadCommand request,
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

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var duplicateExists =
                await _leadRepository
                    .EmailExistsForOtherLeadAsync(
                        _currentUser.TenantId,
                        request.LeadId,
                        request.Email,
                        cancellationToken);

            if (duplicateExists)
            {
                throw new ConflictException(
                    "Lead with the same email already exists.");
            }
        }

        var rowVersion =
            Convert.FromBase64String(
                request.RowVersion);

        _concurrencyService.SetOriginalRowVersion(
            lead,
            rowVersion);

        lead.Update(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Company,
            request.Source,
            request.OwnerUserId);

        try
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (ConcurrencyException)
        {
            throw new ConcurrencyException(
                "The lead was modified by another user. Please refresh and try again.");
        }
    }
}
