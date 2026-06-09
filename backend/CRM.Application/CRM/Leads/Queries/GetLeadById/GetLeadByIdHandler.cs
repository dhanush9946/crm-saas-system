using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Leads.DTOs;
using CRM.Application.CRM.Leads.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Leads.Queries.GetLeadById;

public sealed class GetLeadByIdHandler
    : IRequestHandler<GetLeadByIdQuery, LeadDetailsDto>
{
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUser _currentUser;

    public GetLeadByIdHandler(
        ILeadRepository leadRepository,
        ICurrentUser currentUser)
    {
        _leadRepository = leadRepository;
        _currentUser = currentUser;
    }

    public async Task<LeadDetailsDto> Handle(
        GetLeadByIdQuery request,
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

        return new LeadDetailsDto
        {
            Id = lead.Id,
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Email = lead.Email,
            Phone = lead.Phone,
            Company = lead.Company,
            Source = lead.Source.ToString(),
            Status = lead.Status.ToString(),
            Score = lead.Score,
            ScoreVersion = lead.ScoreVersion,
            OwnerUserId = lead.OwnerUserId,
            CreatedAtUtc = lead.CreatedAtUtc,
            UpdatedAtUtc = lead.UpdatedAtUtc,
            RowVersion = Convert.ToBase64String(
                lead.RowVersion!)
        };
    }
}
