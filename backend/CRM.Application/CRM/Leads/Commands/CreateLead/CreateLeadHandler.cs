using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Leads.Interfaces;
using CRM.Domain.CRM.Entities;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.CreateLead;

public sealed class CreateLeadHandler
    : IRequestHandler<CreateLeadCommand, Guid>
{
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLeadHandler(
        ILeadRepository leadRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _leadRepository = leadRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateLeadCommand request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var exists =
                await _leadRepository.EmailExistsAsync(
                    _currentUser.TenantId,
                    request.Email,
                    cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    "Lead with the same email already exists.");
            }
        }

        var lead = Lead.Create(
            tenantId: _currentUser.TenantId,
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            phone: request.Phone,
            company: request.Company,
            source: request.Source,
            ownerUserId: request.OwnerUserId);

        await _leadRepository.AddAsync(
            lead,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return lead.Id;
    }
}